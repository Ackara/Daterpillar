using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.TextTransformation
{
    /// <summary>
    /// Represents a database indexed column.
    /// </summary>
    public struct IndexColumn
    {
        /// <summary>
        /// Gets or sets the column's name.
        /// </summary>
        /// <value>The name.</value>
        [XmlText]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the column's order.
        /// </summary>
        /// <value>The order.</value>
        [XmlAttribute("order")]
        public SortOrder Order { get; set; }
    }
}