using System.Collections.Generic;
using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a database index.
    /// </summary>
    public class Index : ICloneable<Index>
    {
        public Index()
        {
            Columns = new List<IndexColumn>();
        }

        [XmlIgnore]
        public Table TableRef;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Index" /> is unique.
        /// </summary>
        /// <value><c>true</c> if unique; otherwise, <c>false</c>.</value>
        [XmlAttribute("unique")]
        public bool Unique { get; set; }

        /// <summary>
        /// Gets or sets the type of the index.
        /// </summary>
        /// <value>The type of the index.</value>
        [XmlAttribute("type")]
        public IndexType Type { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the associated columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlElement("columnName")]
        public List<IndexColumn> Columns { get; set; }

        public Index Clone()
        {
            return new Index()
            {
                Name = this.Name,
                Type = this.Type,
                Unique = this.Unique,
                Columns = this.Columns
            };
        }
    }
}