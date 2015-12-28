using SQLitePCL;
using System;

namespace Gigobyte.Daterpillar.Data
{
    public class SQLitePclEntityConstruction : IEntityConstructor
    {
        public object CreateInstance(Type returnType, object state)
        {
            var data = state as ISQLiteStatement;
            if (data != null)
            {
                var entity = (EntityBase)Activator.CreateInstance(returnType);
                object value;

                foreach (var column in entity.GetColumns())
                {
                    value = data[column.Name];
                    if (value == null) continue;
                    else column.SetValue(entity, Convert.ChangeType(value, column.GetPropertyType()));
                }
                return entity;
            }
            else throw new ArgumentException($"The '{nameof(state)}' parameter must be of type '{nameof(ISQLiteStatement)}' not '{state.GetType().Name}'.", nameof(state));
        }
    }
}