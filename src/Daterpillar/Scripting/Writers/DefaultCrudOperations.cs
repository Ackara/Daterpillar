using Acklann.Daterpillar.Modeling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Scripting.Writers
{
    /// <summary>
    /// Defines method that populate SQL values array when constructing an INSERT statement.
    /// </summary>
    /// <param name="context">The method context.</param>
    /// <param name="propertyValue">The value of the target column-member.</param>
    public delegate void SqlValueArrayWriting(SqlValueArrayPluginContext context, object propertyValue);

    public delegate void AfterSqlDataRecordLoaded(object record, IDataRecord data);

    public class DefaultCrudOperations : ICrudOperations
    {
        public void Add(string key, SqlValueArrayWriting plugin)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));

            _createPlugins.Add(key, plugin);
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
            //string[] columns = ColumnMap.GetColumnNames(tableName);
            string[] columns = ColumnMap.GetColumns(tableName).Select(x => x.Item1).ToArray();

            var builder = new StringBuilder();
            builder.Append("INSERT INTO ")
                   .Append(SqlExtensions.EscapeColumn(tableName, dialect))
                   .Append(" (")
                   .Append(string.Join(", ", (from x in columns select SqlExtensions.EscapeColumn(x, dialect))))
                   .Append(")")
                   .Append(" VALUES ")
                   .Append("(")
                   .Append(string.Join(", ", GetValues(record, recordType, tableName, columns.Length)))
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
            var members = ColumnMap.GetNonIdentityColumns(tableName).Distinct().ToArray();
            var keys = ColumnMap.GetIdentityColumns(tableName).Distinct().ToArray();

            IEnumerable<ColumnValuePair> setArray = getSqlArray(record, recordType, members);
            IEnumerable<ColumnValuePair> keyArray = getSqlArray(record, recordType, keys);

            var builder = new StringBuilder();
            builder.Append("UPDATE ")
                   .Append(tableName.EscapeColumn(dialect))
                   .Append(" SET ")
                   .Append(string.Join(", ", setArray.Select(x => $"{x.ColumnName.EscapeColumn(dialect)}={x.Value}")))
                   .Append("WHERE ")
                   .Append(string.Join(", ", keyArray.Select(x => $"{x.ColumnName.EscapeColumn(dialect)}={x.Value}")));
            return builder.ToString();

            IEnumerable<ColumnValuePair> getSqlArray(object record, Type recordType, (string, MemberInfo)[] columns)
            {
                var context = new SqlValueArrayPluginContext { Array = new ColumnValuePair[columns.Length] };

                foreach ((string columnName, MemberInfo member) in columns)
                {
                    string key = CreateKey(recordType.FullName, member.Name);
                    context.Failed = false;

                    if (_createPlugins.TryGetValue(key, out SqlValueArrayWriting plugin))
                    {
                        plugin.Invoke(context, GetValue(member, record));
                        if (context.Failed == false) continue;
                    }

                    context.SetValue(columnName, WriteValue(member, record));
                }

                return context.Array;
            }
        }

        public string Delete(object model, Language dialect)
        {
            throw new NotImplementedException();
        }

        bool ICrudOperations.CanAccept(Type type) => true;

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

        internal static string CreateKey(string typeName, string memberName) => string.Concat(typeName, '.', memberName);

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

        private readonly IDictionary<string, SqlValueArrayWriting> _createPlugins = new Dictionary<string, SqlValueArrayWriting>();
        private readonly IDictionary<string, AfterSqlDataRecordLoaded> _readPlugins = new Dictionary<string, AfterSqlDataRecordLoaded>();

        private IEnumerable<object> GetValues(object record, Type recordType, string tableName, int totalColumns)
        {
            //MemberInfo[] members = ColumnMap.GetMembers(tableName);
            MemberInfo[] members = ColumnMap.GetColumns(tableName).Select(x => x.Item2).Distinct().ToArray();
            var context = new SqlValueArrayPluginContext { Array = new ColumnValuePair[totalColumns] };

            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo member = members[i];
                string key = CreateKey(recordType.FullName, member.Name);
                context.Failed = false;

                if (_createPlugins.TryGetValue(key, out SqlValueArrayWriting plugin))
                {
                    plugin.Invoke(context, GetValue(member, record));
                    if (context.Failed == false) continue;
                }

                context.SetValue(WriteValue(member, record));
            }

            return context.Array.Select(x => x.Value);
        }

        #endregion Backing Members
    }

    public class SqlValueArrayPluginContext
    {
        public int Index;

        public IList<ColumnValuePair> Array;

        public bool Failed;

        public void SetValue(object value) => SetValue(null, value);

        public void SetValue(string columnName, object value)
        {
            Array[Index++] = new ColumnValuePair(columnName, value);
        }
    }
}