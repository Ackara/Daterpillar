using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents an <see cref="Index"/> column reference.
    /// </summary>
    public struct ColumnName : IEquatable<ColumnName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnName"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="order">The order.</param>
        public ColumnName(string name, Order order = Order.ASC)
        {
            Name = name;
            Order = order;
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name.</value>
        [XmlText]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the column's sorting order.
        /// </summary>
        /// <value>The order.</value>
        [XmlAttribute("order")]
        [DefaultValue(Order.ASC)]
        public Order Order { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ColumnName) return Equals((ColumnName)obj);
            else return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ColumnName other)
        {
            return Name == other.Name && Order == other.Order;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Order.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Name} {Order}";
        }

        #region Operator Overrides

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ColumnName left, ColumnName right) => left.Equals(right);

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ColumnName left, ColumnName right) => !left.Equals(right);

        #endregion Operator Overrides
    }
}