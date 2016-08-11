using System;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.TextTransformation
{
    /// <summary>
    /// Represents a SQL type.
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
        public DataType(string typeName) : this()
        {
            Name = typeName;
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

        public bool Equals(DataType other)
        {
            return Name == other.Name && Precision == other.Precision && Scale == other.Scale;
        }

        public override bool Equals(object obj)
        {
            if (obj is DataType) return Equals((DataType)obj);
            else return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Precision.GetHashCode() ^ Scale.GetHashCode();
        }
    }
}