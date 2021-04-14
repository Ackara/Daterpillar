using System.Collections;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    public abstract class DataRecord : DataViewRecord, IInsertable
    {
        public DataRecord()
        {
            if (_map.ContainsKey(GetTableName()) == false)
            {
                PropertyInfo[] properties = GetType().GetColumns().ToArray();

                _map.Add(GetKey(nameof(GetColumns)), from i in properties select i.GetName());
                _map.Add(GetKey(nameof(GetValues)), properties);

                foreach (PropertyInfo property in properties)
                {
                    _map.Add(GetKey(property.GetName()), property);
                }
            }
        }

        public string GetTableName() => TableName;

        public virtual string[] GetColumns()
        {
            return (string[])_map[GetKey(nameof(GetColumns))];
        }

        public virtual object[] GetValues()
        {
            PropertyInfo[] values = ColumnMap.GetProperties(GetTableName());
            object[] results = new object[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                results[i] = Serialize(values[i]);
            }

            return values;
        }

        public virtual object Serialize(PropertyInfo property)
        {
            return Linq.SqlComposer.Serialize(property.GetValue(this));
        }

        #region Backing Members

        private static readonly Hashtable _map = new Hashtable();

        private string GetKey(string item) => string.Concat(GetTableName(), '.', item);

        #endregion Backing Members
    }
}