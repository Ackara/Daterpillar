using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents a SQL data type.
    /// </summary>
    public struct DataType : IEquatable<DataType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataType"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public DataType(SchemaType name, int scale = 0, int precision = 0)
        {
            Name = Translators.TranslatorBase.ConvertToString(name);
            Precision = precision;
            Scale = scale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataType"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        internal DataType(string name, int scale = 0, int precision = 0)
        {
            Name = name;
            Scale = scale;
            Precision = precision;
        }

        /// <summary>
        /// Gets or sets the name of the SQL type.
        /// </summary>
        /// <value>The name.</value>
        [XmlText]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        [XmlAttribute("scale"), DefaultValue(0)]
        public int Scale { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>The precision.</value>
        [XmlAttribute("precision"), DefaultValue(0)]
        public int Precision { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Name}({Scale}, {Precision})";
        }

        #region IEquatable

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DataType left, DataType right) => left.Equals(right);

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DataType left, DataType right) => !left.Equals(right);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DataType) return Equals((DataType)obj);
            else return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(DataType other)
        {
            return Name.Equals(other.Name, StringComparison.CurrentCultureIgnoreCase)
                && Scale == other.Scale
                && Precision == other.Precision;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Precision.GetHashCode() ^ Scale.GetHashCode();
        }

        #endregion IEquatable
    }
}