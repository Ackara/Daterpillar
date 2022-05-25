using Acklann.Daterpillar.Modeling;
using Acklann.Daterpillar.Serialization;
using System;
using System.Collections.Generic;
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
                   .Append(SqlComposer.EscapeColumn(tableName, dialect))
                   .Append(" (")
                   .Append(string.Join(", ", (from x in ColumnMap.GetColumns(tableName) select SqlComposer.EscapeColumn(x, dialect))))
                   .Append(")")
                   .Append(" VALUES ")
                   .Append("(")
                   .Append(string.Join(", ", (from x in GetValues(tableName) select x)))
                   .Append(")");

            return builder.ToString();
        }

        public IEnumerable<object> Read(string query, Language dialect)
        {
            throw new NotImplementedException();
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

        protected virtual object WriteValue(PropertyInfo member)
        {
            return SqlComposer.Serialize(member.GetValue(this));
        }

        protected virtual object WriteValue(FieldInfo member)
        {
            return SqlComposer.Serialize(member.GetValue(this));
        }

        protected object WriteValue(MemberInfo member)
        {
            if (member is PropertyInfo property)
                return WriteValue(property);
            else if (member is FieldInfo field)
                return WriteValue(field);
            else
                return null;
        }

        #region Backing Members

        private object[] GetValues(string tableName)
        {
            MemberInfo[] members = ColumnMap.GetMembers(tableName);
            object[] results = new object[members.Length];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = WriteValue(members[i]);
            }

            return results;
        }

        #endregion Backing Members
    }
}