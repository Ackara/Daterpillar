using System;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// Indicates that a public field or property represents a SQL table column. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage((AttributeTargets.Property), AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        public ColumnAttribute() : this(null, null, 0, 0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ColumnAttribute(string name) : this(name, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public ColumnAttribute(string typeName, int scale = 0, int precision = 0) : this(null, typeName, scale, precision)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public ColumnAttribute(string name, string typeName, int scale = 0, int precision = 0)
        {
            Name = name;
            DataType = new DataType(typeName, scale, precision);
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The data type.
        /// </summary>
        public readonly DataType DataType;

        /// <summary>
        /// Indicates whether the column is auto incremented.
        /// </summary>
        public bool AutoIncrement;

        /// <summary>
        /// Indicates whether the column can be set to <c>null</c>.
        /// </summary>
        public bool Nullable;

        /// <summary>
        /// The default value.
        /// </summary>
        public string DefaultValue;
    }
}