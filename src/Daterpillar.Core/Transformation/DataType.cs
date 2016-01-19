using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    /// <summary>
    /// Represents a SQL type.
    /// </summary>
    public struct DataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataType"/> struct.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        public DataType(string typeName) : this()
        {
            Name = typeName;
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        [XmlAttribute("scale")]
        public int Scale { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        [XmlAttribute("precision")]
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlText]
        public string Name { get; set; }
    }
}