using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// Represents a <see cref="Schema"/> table.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.ISQLObject" />
    public class Table
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        public Table() : this(null, new Column[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlObjects">The SQL objects.</param>
        public Table(string name, params object[] sqlObjects)
        {
            Name = name;
            Columns = new List<Column>();
            Indexes = new List<Index>();
            Constraints = new List<ForeignKey>();

            foreach (var item in sqlObjects)
            {
                if (item is Column column)
                {
                    column.Table = this;
                    Columns.Add(column);
                }
                else if (item is Index index)
                {
                    index.Table = this;
                    Indexes.Add(index);
                }
                else if (item is ForeignKey fKey)
                {
                    fKey.Table = this;
                    Constraints.Add(fKey);
                }
            }
        }

        /// <summary>
        /// The parent schema.
        /// </summary>
        [XmlIgnore]
        public Schema Schema;

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table columns.
        /// </summary>
        /// <value>The table columns.</value>
        [XmlElement("column")]
        public List<Column> Columns { get; set; }

        /// <summary>
        /// Gets or sets the table foreign keys.
        /// </summary>
        /// <value>The table foreign keys.</value>
        [XmlElement("foreignKey")]
        public List<ForeignKey> Constraints { get; set; }

        /// <summary>
        /// Gets or sets the table indexes.
        /// </summary>
        /// <value>The table indexes.</value>
        [XmlElement("index")]
        public List<Index> Indexes { get; set; }
    }
}