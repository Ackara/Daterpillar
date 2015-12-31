using System.Reflection;

namespace Gigobyte.Daterpillar.Data
{
    /// <summary>
    /// Represent the current values and meta data of a property member.
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        internal ColumnInfo(PropertyInfo propertyInfo)
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

        /// <summary>
        /// Gets the <see cref="PropertyInfo.PropertyType"/> value.
        /// </summary>
        /// <returns></returns>
        public System.Type GetPropertyType()
        {
            return _propertyInfo.PropertyType;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo.GetValue(object)"/> value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public object GetValue(object obj)
        {
            return _propertyInfo.GetValue(obj);
        }

        /// <summary>
        /// Invoke the <see cref="PropertyInfo.SetValue(object, object)"/> method.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object value)
        {
            _propertyInfo.SetValue(obj, value);
        }

        #region Private Member

        private PropertyInfo _propertyInfo;

        #endregion Private Member
    }
}