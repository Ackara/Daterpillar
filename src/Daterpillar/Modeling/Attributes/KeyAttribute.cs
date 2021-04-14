using System;

namespace Acklann.Daterpillar.Modeling.Attributes
{
    /// <summary>
    /// Indicates a column is a key. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class KeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
        /// </summary>
        public KeyAttribute() : this(Order.ASC)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
        /// </summary>
        /// <param name="order">The sorting order.</param>
        public KeyAttribute(Order order)
        {
            Order = order;
        }

        /// <summary>
        /// The sorting order
        /// </summary>
        public readonly Order Order;
    }
}