using System;
using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a SQL data type.
    /// </summary>
    public struct DataType : IEquatable<DataType>
    {
        #region Operators

        public static bool operator ==(DataType left, DataType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataType left, DataType right)
        {
            return !left.Equals(right);
        }

        #endregion Operators

        /// <summary>
        /// Initializes a new instance of the <see cref="DataType"/> struct.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        public DataType(string typeName) : this(typeName, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataType"/> struct.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="scale">The scale.</param>
        public DataType(string typeName, int scale) : this(typeName, scale, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataType"/> struct.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public DataType(string typeName, int scale, int precision)
        {
            Name = typeName;
            Scale = scale;
            Precision = precision;
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        [XmlAttribute("scale")]
        public int Scale { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>The precision.</value>
        [XmlAttribute("precision")]
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlText]
        public string Name { get; set; }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="DataType"/> object have
        /// the same value.
        /// </summary>
        /// <param name="other">The instance to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of the parameter is the same as this instance; <c>false</c> otherwise.
        /// </returns>
        public bool Equals(DataType other)
        {
            return Name == other.Name && Precision == other.Precision && Scale == other.Scale;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is DataType) return Equals((DataType)obj);
            else return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Precision.GetHashCode() ^ Scale.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Name}({Scale}, {Precision})";
        }
    }
}