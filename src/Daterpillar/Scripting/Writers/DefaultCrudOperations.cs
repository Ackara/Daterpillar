using Acklann.Daterpillar.Modeling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public class DefaultCrudOperations : ICrudOperations
    {
        public void Add(string key, SqlValueArrayBuilding plugin)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));

            _sqlArrayBuilderPlugins.Add(key, plugin);
        }

        public void Add(string key, AfterSqlDataRecordLoaded plugin)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));

            _readPlugins.Add(key, plugin);
        }

        public string Create(object record, Language dialect = Language.SQL)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ColumnMap.Register(recordType);
            string tableName = recordType.GetTableName();
            ColumnMemberPair[] columns = ColumnMap.GetColumns(tableName);

            var builder = new StringBuilder();
            builder.Append("INSERT INTO ")
                   .Append(SqlExtensions.EscapeColumn(tableName, dialect))
                   .Append(" (")
                   .Append(string.Join(", ", (from x in columns select SqlExtensions.EscapeColumn(x.ColumnName, dialect))))
                   .Append(")")
                   .Append(" VALUES ")
                   .Append("(")
                   .Append(string.Join(", ", BuildValueArray(record, recordType, columns).Select(x => x.Value)))
                   .Append(")");

            return builder.ToString();
        }

        public object Read(IDataRecord data, Type recordType)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            object record = Activator.CreateInstance(recordType);
            string tableName = recordType.GetTableName();
            int nColumns = data.FieldCount;

            for (int i = 0; i < nColumns; i++)
            {
                TryReadDataRow(data, record, ColumnMap.GetMember(tableName, data.GetName(i)), data.GetValue(i));
            }

            if (_readPlugins.TryGetValue(recordType.FullName, out AfterSqlDataRecordLoaded plugin))
            {
                plugin.Invoke(record, data);
            }

            return record;
        }

        public string Update(object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ColumnMap.Register(recordType);
            string tableName = recordType.GetTableName();
            ColumnValuePair[] setArray = BuildValueArray(record, recordType, ColumnMap.GetNonIdentityColumns(tableName));
            ColumnValuePair[] whereArray = BuildValueArray(record, recordType, ColumnMap.GetIdentityColumns(tableName));

            var builder = new StringBuilder();
            builder.Append("UPDATE ")
                   .Append(tableName.EscapeColumn(dialect))
                   .Append(" SET ")
                   .Append(string.Join(",", setArray.Select(x => $"{x.ColumnName.EscapeColumn(dialect)}={x.Value}")))
                   .Append(" WHERE ")
                   .Append(string.Join(",", whereArray.Select(x => $"{x.ColumnName.EscapeColumn(dialect)}={x.Value}")));
            return builder.ToString();
        }

        public string Delete(object model, Language dialect)
        {
            throw new NotImplementedException();
        }

        bool ICrudOperations.CanAccept(Type type) => true;

        internal static string CreateKey(string typeName, string memberName) => string.Concat(typeName, '.', memberName);

        protected virtual bool TryReadDataRow(IDataRecord results, object record, MemberInfo member, object value)
        {
            try
            {
                if (member is PropertyInfo property)
                    ReadDataRow(record, property, Convert.ChangeType(value, property.PropertyType), results);
                else if (member is FieldInfo field)
                    ReadDataRow(record, field, Convert.ChangeType(value, field.FieldType), results);
                return true;
            }
            catch (InvalidCastException ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
            catch (ArgumentException ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
            return false;
        }

        protected virtual void ReadDataRow(object instance, PropertyInfo member, object value, IDataRecord record)
        {
            if (value != DBNull.Value) member?.SetValue(instance, value);
        }

        protected virtual void ReadDataRow(object instance, FieldInfo member, object value, IDataRecord record)
        {
            if (value != DBNull.Value) member?.SetValue(instance, value);
        }

        protected virtual object WriteValue(PropertyInfo member, object record)
        {
            return SqlExtensions.Serialize(member.GetValue(record));
        }

        protected virtual object WriteValue(FieldInfo member, object record)
        {
            return SqlExtensions.Serialize(member.GetValue(record));
        }

        protected object WriteValue(MemberInfo member, object instance)
        {
            if (member is PropertyInfo property)
                return WriteValue(property, instance);
            else if (member is FieldInfo field)
                return WriteValue(field, instance);
            else
                return null;
        }

        protected object GetValue(MemberInfo member, object instance)
        {
            if (member is PropertyInfo property)
                return property.GetValue(instance);
            else if (member is FieldInfo field)
                return field.GetValue(instance);
            else
                return null;
        }

        #region Backing Members

        private readonly IDictionary<string, SqlValueArrayBuilding> _sqlArrayBuilderPlugins = new Dictionary<string, SqlValueArrayBuilding>();
        private readonly IDictionary<string, AfterSqlDataRecordLoaded> _readPlugins = new Dictionary<string, AfterSqlDataRecordLoaded>();

        private ColumnValuePair[] BuildValueArray(object record, Type recordType, ColumnMemberPair[] columns)
        {
            var context = new SqlValueArrayPluginContext(columns);

            foreach (ColumnMemberPair pair in columns.Distinct(new ColumnMemberPairEqualityComparer()))
            {
                string key = CreateKey(recordType.FullName, pair.MemberName);
                context.Failed = false;

                if (_sqlArrayBuilderPlugins.TryGetValue(key, out SqlValueArrayBuilding plugin))
                {
                    plugin.Invoke(context, record);
                    if (context.Failed == false) continue;
                }

                context.SetValue(WriteValue(pair.Member, record));
            }

            return context.Array;
        }

        #endregion Backing Members
    }

    public delegate void AfterSqlDataRecordLoaded(object record, IDataRecord data);

    /// <summary>
    /// Defines method that populate SQL values array when constructing an INSERT statement.
    /// </summary>
    /// <param name="context">The method context.</param>
    /// <param name="record">An instance of the record.</param>
    public delegate void SqlValueArrayBuilding(SqlValueArrayPluginContext context, object record);

    public class SqlValueArrayPluginContext
    {
        internal SqlValueArrayPluginContext(int capacity)
        {
            Array = new ColumnValuePair[capacity];
        }

        internal SqlValueArrayPluginContext(ColumnMemberPair[] columns)
        {
            Array = new ColumnValuePair[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                Array[i] = new ColumnValuePair(columns[i].ColumnName, null);
            }
        }

        public bool Failed;

        public int Index;

        public ColumnValuePair[] Array;

        public void SetValue(object value)
        {
            Array[Index++].Value = value;
        }
    }
}