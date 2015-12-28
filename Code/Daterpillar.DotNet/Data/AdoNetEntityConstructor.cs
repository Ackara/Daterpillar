using System;
using System.Data;

namespace Gigobyte.Daterpillar.Data
{
    public class AdoNetEntityConstructor : IEntityConstructor
    {
        public object CreateInstance(Type returnType, object state)
        {
            var data = state as DataRow;
            if (data != null)
            {
                var entity = (EntityBase)Activator.CreateInstance(returnType);
                object value;
                foreach (var column in entity.GetColumns())
                {
                    value = data[column.Name];
                    if (value == DBNull.Value) continue;
                    else column.SetValue(entity, Convert.ChangeType(value, column.GetPropertyType()));
                }
                return entity;
            }
            else throw new ArgumentException($"The '{nameof(state)}' parameter must be of type '{nameof(DataRow)}' not '{state.GetType().Name}'.", nameof(state));
        }
    }
}