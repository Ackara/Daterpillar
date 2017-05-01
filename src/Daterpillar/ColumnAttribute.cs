using System;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// Indicates that a public field or property represents a SQL table column. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage((AttributeTargets.Property | AttributeTargets.Field), AllowMultiple = false, Inherited = true)]
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
        /// The name of the column.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Indicates whether the column is auto incremented.
        /// </summary>
        public bool AutoIncrement;

        /// <summary>
        /// Indicates whether the column can be set to <c>null</c>.
        /// </summary>
        public bool Nullable;
    }
}