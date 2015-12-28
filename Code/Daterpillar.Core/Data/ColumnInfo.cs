using System.Reflection;

namespace Gigobyte.Daterpillar.Data
{
    public class ColumnInfo
    {
        public ColumnInfo(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets whether the column's primary key is auto incremented.
        /// </summary>
        public bool AutoIncremented { get; internal set; }

        /// <summary>
        /// Get whether the column is or one part of a primary key.
        /// </summary>
        public bool IsKey { get; internal set; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the value assigned in the column.
        /// </summary>
        public object Value { get; internal set; }

        public System.Type GetPropertyType()
        {
            return _propertyInfo.PropertyType;
        }

        public object GetValue(object obj)
        {
            return _propertyInfo.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            _propertyInfo.SetValue(obj, value);
        }

        #region Private Member

        private PropertyInfo _propertyInfo;

        #endregion Private Member
    }
}