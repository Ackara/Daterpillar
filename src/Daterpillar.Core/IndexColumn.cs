using System.Xml.Serialization;

namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Represents a database indexed column.
    /// </summary>
    public struct IndexColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexColumn"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        public IndexColumn(string name) : this(name, SortOrder.ASC)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexColumn"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="order">The order.</param>
        public IndexColumn(string name, SortOrder order)
        {
            Name = name;
            Order = order;
        }

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