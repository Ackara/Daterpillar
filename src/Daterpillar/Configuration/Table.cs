using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents a <see cref="Schema"/> table.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class Table : ISQLObject
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
        public Table(string name, params ISQLObject[] sqlObjects)
        {
            Name = name;
            Columns = new List<Column>();
            Indecies = new List<Index>();
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
                    Indecies.Add(index);
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
        [XmlIgnore, IgnoreDataMember]
        public Schema Schema;

        [XmlAttribute("suid"), DefaultValue(0)]
        public int Id { get; set; }

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
        [XmlElement("documentation")]
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
        public List<Index> Indecies { get; set; }

        [IgnoreDataMember, XmlIgnore]
        internal int Weight
        {
            get { return ForeignKeys?.Count ?? 0; }
        }

        public void Merge(Table table)
        {
            foreach (Column right in table.Columns)
            {
                Column left = Columns.Find(l => l.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase));
                if (left == null)
                    Columns.Add(right);
                else
                    left.Overwrite(right);
            }

            foreach (ForeignKey right in table.ForeignKeys)
            {
                ForeignKey left = ForeignKeys.Find(l => l.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase));
                if (left == null)
                    ForeignKeys.Add(right);
                else
                    left.Overwrite(right);
            }

            foreach (Index right in table.Indecies)
            {
                Index left = Indecies.Find(l => l.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase));
                if (left == null)
                    Indecies.Add(right);
                else
                    left.Overwrite(right);
            }
        }

        string ISQLObject.GetName() => Name;

        #region ICloneable

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

            foreach (var idx in Indecies)
            {
                var copy = idx.Clone();
                copy.Table = clone;
                clone.Indecies.Add(copy);
            }

            foreach (var fKey in ForeignKeys)
            {
                var copy = fKey.Clone();
                copy.Table = clone;
                clone.ForeignKeys.Add(copy);
            }

            return clone;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone() => Clone();

        #endregion ICloneable

        #region Private Members

        private Column Find(Column right)
        {
            throw new System.NotImplementedException();
        }

        #endregion Private Members
    }
}