using Acklann.Daterpillar.Serialization;
using System.Data;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    public abstract class DataViewRecord : ISelectable
    {
        public DataViewRecord()
        {
            TableName = GetType().GetTableName();
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
    }
}