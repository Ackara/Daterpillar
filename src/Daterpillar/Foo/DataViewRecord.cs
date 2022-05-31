using Acklann.Daterpillar.Modeling;
using Acklann.Daterpillar.Scripting;
using System;
using System.Data;
using System.Reflection;

namespace Acklann.Daterpillar.Foo
{
    public abstract class DataViewRecord : ISelectable
    {
        public DataViewRecord() : this(null)
        {
        }

        public DataViewRecord(Type type)
        {
            TableName = (type ?? GetType()).GetTableName();
        }

        protected internal readonly string TableName;

        public virtual void Load(IDataRecord data)
        {
            int n = data.FieldCount;
            for (int i = 0; i < n; i++)
            {
                ReadDataRow(ColumnMap.GetMember(TableName, data.GetName(i)), data.GetValue(i), data);
            }
        }

        protected virtual void ReadDataRow(PropertyInfo member, object value, IDataRecord record)
        {
            if (value != System.DBNull.Value)
                member?.SetValue(this, value);
        }

        protected virtual void ReadDataRow(FieldInfo member, object value, IDataRecord record)
        {
            if (value != System.DBNull.Value)
                member?.SetValue(this, value);
        }

        private void ReadDataRow(MemberInfo member, object value, IDataRecord record)
        {
            if (member is PropertyInfo property)
                ReadDataRow(property, value, record);
            else if (member is FieldInfo field)
                ReadDataRow(field, value, record);
        }
    }
}