using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a database table.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    public sealed class Table : ICloneable<Table>
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

            Comment = string.Empty;
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
            return CreateColumn(name, new DataType("VARCHAR", 64, 0));
        }

        public Column CreateColumn(string name, DataType type, bool autoIncrement = false, bool nullable = false, string comment = "")
        {
            var newColumn = new Column()
            {
                TableRef = this,

                Name = name,
                DataType = type,
                Comment = comment,
                IsNullable = nullable,
                AutoIncrement = autoIncrement
            };
            Columns.Add(newColumn);
            newColumn.OrdinalPosition = Columns.Count;

            return newColumn;
        }

        public Index CreateIndex()
        {
            var newIndex = new Index() { TableRef = this };
            Indexes.Add(newIndex);

            return newIndex;
        }

        public Index CreateIndex(string name, IndexType type, bool unique, params IndexColumn[] columns)
        {
            var newIndex = new Index()
            {
                TableRef = this,

                Name = name,
                Type = type,
                Unique = unique,
                Columns = columns.ToList()
            };

            Indexes.Add(newIndex);
            return newIndex;
        }

        public ForeignKey CreateForeignKey()
        {
            var newKey = new ForeignKey() { TableRef = this };
            ForeignKeys.Add(newKey);

            return newKey;
        }

        public ForeignKey CreateForeignKey(string column, string foreignTable, string foreignColumn, ForeignKeyRule onUpdate = ForeignKeyRule.CASCADE, ForeignKeyRule onDelete = ForeignKeyRule.CASCADE, string name = null)
        {
            var newConstraint = new ForeignKey()
            {
                TableRef = this,
                LocalTable = Name,
                LocalColumn = column,
                ForeignTable = foreignTable,
                ForeignColumn = foreignColumn,
                OnUpdate = onUpdate,
                OnDelete = onDelete
            };
            newConstraint.Name = newConstraint.GetName((ForeignKeys.Count + 1));

            ForeignKeys.Add(newConstraint);
            return newConstraint;
        }

        public bool RemoveColumn(string name)
        {
            for (int i = 0; i < Columns.Count; i++)
                if (Columns[i].Name == name)
                {
                    Columns.RemoveAt(i);
                    return true;
                }

            return false;
        }

        public bool RemoveIndex(string name)
        {
            for (int i = 0; i < Indexes.Count; i++)
                if (Indexes[i].Name == name)
                {
                    Indexes.RemoveAt(i);
                    return true;
                }

            return false;
        }

        public bool RemoveForeignKey(string name)
        {
            for (int i = 0; i < ForeignKeys.Count; i++)
                if (ForeignKeys[i].Name == name)
                {
                    ForeignKeys.RemoveAt(i);
                    return true;
                }

            return false;
        }

        public Table Clone()
        {
            var clone = new Table();
            clone.Name = Name;
            clone.Comment = Comment;

            foreach (var column in Columns)
            {
                var copy = column.Clone();
                clone.Columns.Add(copy);
                copy.TableRef = clone;
            }

            foreach (var constraint in ForeignKeys)
            {
                ForeignKey copy = constraint.Clone();
                clone.ForeignKeys.Add(copy);
                copy.TableRef = clone;
            }

            foreach (var index in Indexes)
            {
                Index copy = index.Clone();
                clone.Indexes.Add(copy);
                copy.TableRef = clone;
            }

            return clone;
        }

        #region Private Members

        private string ToDebuggerDisplay()
        {
            return $"{Name}";
        }

        #endregion Private Members
    }
}