using System;

namespace Acklann.Daterpillar.Modeling.Attributes
{
    /// <summary>
    /// Indicates that a public field or property represents a SQL index. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage((AttributeTargets.Property | AttributeTargets.Field), AllowMultiple = true, Inherited = true)]
    public sealed class IndexAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        public IndexAttribute() : this(string.Empty, IndexType.Index)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public IndexAttribute(IndexType type) : this(string.Empty, type)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public IndexAttribute(string name, IndexType type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// The index type.
        /// </summary>
        public readonly IndexType Type;

        /// <summary>
        /// Indexes with the same group name will be treated as a composite index.
        /// Use the it to compose multiple columns under on index.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Indicates whether the index is unique.
        /// </summary>
        /// <value>
        ///   <c>true</c> if unique; otherwise, <c>false</c>.
        /// </value>
        public bool Unique { get; set; }

        /// <summary>
        /// Gets or sets the sorting order.
        /// </summary>
        /// <value>
        /// The sorting order.
        /// </value>
        public Order Order { get; set; }
    }
}