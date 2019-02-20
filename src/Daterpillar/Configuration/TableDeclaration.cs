using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents a <see cref="Schema"/> table.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class TableDeclaration : ISqlStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDeclaration"/> class.
        /// </summary>
        public TableDeclaration() : this(null, new ColumnDeclaration[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDeclaration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlObjects">The SQL objects.</param>
        public TableDeclaration(string name, params ISqlStatement[] sqlObjects)
        {
            Name = name;
            Columns = new List<ColumnDeclaration>();
            Indecies = new List<Index>();
            ForeignKeys = new List<ForeignKey>();

            foreach (var item in sqlObjects)
            {
                if (item is ColumnDeclaration column)
                {
                    column.Table = this;
                    Columns.Add(column);
                }
                else if (item is Index index)
                {
                    index.Table = this;
                    index.SetName();
                    Indecies.Add(index);
                }
                else if (item is ForeignKey fKey)
                {
                    fKey.Table = this;
                    fKey.SetName();
                    ForeignKeys.Add(fKey);
                }
            }
        }

        /// <summary>
        /// The parent schema.
        /// </summary>
        [XmlIgnore]
        public SchemaDeclaration Schema;

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
        public List<ColumnDeclaration> Columns { get; set; }

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

        public void Add(ColumnDeclaration column)
        {
            column.Table = this;
            Columns.Add(column);
        }

        public void Add(ForeignKey foreignKey)
        {
            foreignKey.Table = this;
            ForeignKeys.Add(foreignKey);
        }

        public void Merge(TableDeclaration table)
        {
            foreach (ColumnDeclaration right in table.Columns)
            {
                ColumnDeclaration left = Columns.Find(l => l.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase));
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

        string ISqlStatement.GetName() => Name;

        #region ICloneable

        /// <summary>
        /// Creates a new <see cref="TableDeclaration"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="TableDeclaration"/> object that is a copy of this instance.</returns>
        public TableDeclaration Clone()
        {
            var clone = new TableDeclaration()
            {
                Id = Id,
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

        #region Internal Members

        internal void RemoveColumn(string name)
        {
            Columns.Remove(
                Columns.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
                );
        }

        internal void RemoveForeignKey(string name)
        {
            ForeignKeys.Remove(
                ForeignKeys.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
                );
        }

        internal void RemoveIndex(string name)
        {
            Indecies.Remove(
                Indecies.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
                );
        }

        #endregion Internal Members
    }
}