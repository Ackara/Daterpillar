using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    /// <summary>Represents a database table.</summary>
    public class Table
    {
        public Table()
        {
            Columns = new List<Column>();
            Modifiers = new List<string>();
            ForeignKeys = new List<ForeignKey>();
            Indexes = new List<Index>();
        }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the comment.</summary>
        /// <value>The comment.</value>
        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>Gets or sets the modifiers.</summary>
        /// <value>The modifiers.</value>
        [XmlElement("modifier")]
        public List<string> Modifiers { get; set; }

        /// <summary>Gets or sets the columns.</summary>
        /// <value>The columns.</value>
        [XmlElement("column")]
        public List<Column> Columns { get; set; }

        /// <summary>Gets or sets the foreign keys.</summary>
        /// <value>The foreign keys.</value>
        [XmlElement("foreignKey")]
        public List<ForeignKey> ForeignKeys { get; set; }

        /// <summary>Gets or sets the indexes.</summary>
        /// <value>The indexes.</value>
        [XmlElement("index")]
        public List<Index> Indexes { get; set; }
    }
}