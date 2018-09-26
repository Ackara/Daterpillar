using Acklann.Daterpillar.Configuration;
using System;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Indicates that a public field or property represents a SQL index. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage((AttributeTargets.Property), AllowMultiple = false, Inherited = true)]
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
        public string Name;

        /// <summary>
        /// Indicates whether the index is unique.
        /// </summary>
        public bool Unique;
    }
}