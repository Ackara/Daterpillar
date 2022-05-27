using Acklann.Daterpillar.Foo;
using Acklann.Daterpillar.Modeling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public class CRUDWriterBase : ICRUDWriter
    {
        public string Create(object record, Language dialect = Language.SQL)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ColumnMap.Register(recordType);
            string tableName = recordType.GetTableName();

            var builder = new StringBuilder();
            builder.Append("INSERT INTO ")
                   .Append(SqlExtensions.EscapeColumn(tableName, dialect))
                   .Append(" (")
                   .Append(string.Join(", ", (from x in ColumnMap.GetColumns(tableName) select SqlExtensions.EscapeColumn(x, dialect))))
                   .Append(")")
                   .Append(" VALUES ")
                   .Append("(")
                   .Append(string.Join(", ", (from x in GetValues(record, tableName) select x)))
                   .Append(")");

            return builder.ToString();
        }

        public IEnumerable<object> Read(string query, Language dialect)
        {
            throw new NotImplementedException();
        }

        public object Read(IDataRecord data, Type recordType)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            object record = Activator.CreateInstance(recordType);
            string tableName = recordType.GetTableName();

            int nColumns = data.FieldCount;
            for (int i = 0; i < nColumns; i++)
            {
                ReadDataRow(record, ColumnMap.GetMember(tableName, data.GetName(i)), data.GetValue(i), data);
            }

            return record;
        }

        public string Update(object model, Language dialect)
        {
            throw new NotImplementedException();
        }

        public string Delete(object model, Language dialect)
        {
            throw new NotImplementedException();
        }

        public bool CanAccept(Type type) => true;

        protected virtual void ReadDataRow(object instance, MemberInfo member, object value, IDataRecord record)
        {
            if (member is PropertyInfo property)
                ReadDataRow(instance, property, value, record);
            else if (member is FieldInfo field)
                ReadDataRow(instance, field, value, record);
        }

        protected virtual void ReadDataRow(object instance, PropertyInfo member, object value, IDataRecord record)
        {
            if (value != DBNull.Value)
                member?.SetValue(instance, value);
        }

        protected virtual void ReadDataRow(object instance, FieldInfo member, object value, IDataRecord record)
        {
            if (value != DBNull.Value)
                member?.SetValue(instance, value);
        }

        protected virtual object WriteValue(PropertyInfo member, object instance)
        {
            return SqlExtensions.Serialize(member.GetValue(instance));
        }

        protected virtual object WriteValue(FieldInfo member, object instance)
        {
            return SqlExtensions.Serialize(member.GetValue(instance));
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

        #region Backing Members

        private object[] GetValues(object instance, string tableName)
        {
            MemberInfo[] members = ColumnMap.GetMembers(tableName);
            object[] results = new object[members.Length];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = WriteValue(members[i], instance);
            }

            return results;
        }

        #endregion Backing Members
    }
}