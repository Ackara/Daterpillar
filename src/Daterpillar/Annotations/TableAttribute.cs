using System;

namespace Acklann.Daterpillar.Annotations
{
    /// <summary>
    /// Indicates that a class or enum represents a SQL table. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum), AllowMultiple = false, Inherited = false)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        public TableAttribute() : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TableAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The table name.
        /// </summary>
        public readonly string Name;
    }
}