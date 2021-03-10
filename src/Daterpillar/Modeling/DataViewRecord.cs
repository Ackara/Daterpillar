using System.Data;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    public abstract class DataViewRecord : IReadable
    {
        public DataViewRecord()
        {
            ColumnMap.Register(GetType());
        }

        public virtual void Load(IDataRecord data)
        {
            int n = data.FieldCount;
            for (int i = 0; i < n; i++)
            {
                ReadDataRow(ColumnMap.GetMember(data.GetName(i)), data.GetValue(i), data);
            }
        }

        protected virtual void ReadDataRow(PropertyInfo member, object value, IDataRecord record)
        {
            member?.SetValue(this, value);
        }
    }
}