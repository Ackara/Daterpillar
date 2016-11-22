using System;
using System.Data;

namespace Acklann.Daterpillar.Data
{
    /// <summary>
    /// Create a <see cref="EntityBase"/> object from a <see cref="System.Data.DataRow"/> object.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Data.IDataReader"/>
    public class AdoNetDataReader : IDataReader
    {
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
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
                    if (value == null || value == DBNull.Value) continue;
                    else column.SetValue(entity, Convert.ChangeType(value, column.GetPropertyType()));
                }
                return entity;
            }
            else throw new ArgumentException($"The '{nameof(state)}' parameter must be of type '{nameof(DataRow)}' not '{state.GetType().Name}'.", nameof(state));
        }
    }
}