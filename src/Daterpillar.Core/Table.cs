﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Represents a database table.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    public sealed class Table
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        public Table() : this(string.Empty, new Column[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Table(string name) : this(name, new Column[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="columns">The columns.</param>
        public Table(string name, IEnumerable<Column> columns)
        {
            Name = name;
            Columns = new List<Column>(columns);
            ForeignKeys = new List<ForeignKey>();
            Modifiers = new List<string>();
            Indexes = new List<Index>();
        }

        /// <summary>
        /// The schema reference
        /// </summary>
        [XmlIgnore]
        public Schema SchemaRef;

        /// <summary>
        /// Gets or sets the name.
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
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        [XmlElement("modifier")]
        public List<string> Modifiers { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlElement("column")]
        public List<Column> Columns { get; set; }

        /// <summary>
        /// Gets or sets the foreign keys.
        /// </summary>
        /// <value>The foreign keys.</value>
        [XmlElement("foreignKey")]
        public List<ForeignKey> ForeignKeys { get; set; }

        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        [XmlElement("index")]
        public List<Index> Indexes { get; set; }

        public Column CreateColumn()
        {
            return CreateColumn("", new DataType("INTEGER", 32, 0), false, false);
        }

        public Column CreateColumn(string name)
        {
            return CreateColumn(name, new DataType("INTEGER", 32, 0), false, false);
        }

        public Column CreateColumn(string name, DataType type, bool autoIncrement = false, bool nullable = false)
        {
            var newColumn = new Column()
            {
                TableRef = this,
                Name = name,
                DataType = type,
                AutoIncrement = autoIncrement,
                IsNullable = nullable
            };
            Columns.Add(newColumn);

            return newColumn;
        }

        public Index CreateIndex()
        {
            var newIndex = new Index() { TableRef = this };
            Indexes.Add(newIndex);

            return newIndex;
        }

        public ForeignKey CreateForeignKey()
        {
            var newKey = new ForeignKey() { TableRef = this };
            ForeignKeys.Add(newKey);

            return newKey;
        }

        #region Private Members

        private string ToDebuggerDisplay()
        {
            return $"{Name}";
        }

        #endregion Private Members
    }
}