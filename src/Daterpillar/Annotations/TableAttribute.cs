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
        public TableAttribute() : this(null, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="optInProperties">if set to <c>true</c> all properties will be treated as columns.</param>
        public TableAttribute(string name, bool optInProperties = false)
        {
            Name = name;
            OptInProperties = optInProperties;
        }

        /// <summary>
        /// The table name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Determine whether to treat all properties as SQL columns.
        /// </summary>
        public readonly bool OptInProperties;
    }
}