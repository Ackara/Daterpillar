using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// Represents a <see cref="Schema"/> table.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.ISQLObject" />
    [System.Diagnostics.DebuggerDisplay("{AsDebuggerDisplay()}")]
    public sealed class Table : ICloneable<Table>
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
            ForeignKeys = new List<ForeignKey>();

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
                    ForeignKeys.Add(fKey);
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
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        [XmlElement("comment")]
        public string Comment { get; set; }

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
        public List<ForeignKey> ForeignKeys { get; set; }

        /// <summary>
        /// Gets or sets the table indexes.
        /// </summary>
        /// <value>The table indexes.</value>
        [XmlElement("index")]
        public List<Index> Indexes { get; set; }

        /// <summary>
        /// Creates a new <see cref="Table"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="Table"/> object that is a copy of this instance.</returns>
        public Table Clone()
        {
            var clone = new Table()
            {
                Name = this.Name,
                Comment = this.Comment,
            };

            foreach (var col in Columns)
            {
                var copy = col.Clone();
                copy.Table = clone;
                clone.Columns.Add(copy);
            }

            foreach (var idx in Indexes)
            {
                var copy = idx.Clone();
                copy.Table = clone;
                clone.Indexes.Add(copy);
            }

            foreach (var fKey in ForeignKeys)
            {
                var copy = fKey.Clone();
                copy.Table = clone;
                clone.ForeignKeys.Add(copy);
            }

            return clone;
        }

        private string AsDebuggerDisplay()
        {
            return $"{Name}";
        }
    }
}