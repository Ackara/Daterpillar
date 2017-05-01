using System;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// Indicates that a public field or property represents a SQL index. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage((AttributeTargets.Property | AttributeTargets.Field), AllowMultiple = false, Inherited = true)]
    public sealed class IndexAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public IndexAttribute(IndexType type)
        {
            Type = type;
        }

        /// <summary>
        /// The index type.
        /// </summary>
        public readonly IndexType Type;

        /// <summary>
        /// Indicates whether the index is unique.
        /// </summary>
        public bool Unique;
    }
}