using System;

namespace Gigobyte.Daterpillar.Data
{
    /// <summary>
    /// Represents the database column that a property is mapped to.
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the column the property is mapped to.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a key.
        /// </summary>
        /// <value><c>true</c> if this instance is key; otherwise, <c>false</c>.</value>
        public bool IsKey { get; set; }

        /// <summary>
        /// Gets or sets whether the column is auto incremented.
        /// </summary>
        /// <value><c>true</c> if [automatic increment]; otherwise, <c>false</c>.</value>
        public bool AutoIncrement { get; set; }
    }
}