using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents a <see cref="Table"/> index.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    public class Index : ISchemaObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// </summary>
        public Index() : this(IndexType.Index, false, new ColumnName[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// </summary>
        /// <param name="type">The index type.</param>
        /// <param name="columns">The column names.</param>
        public Index(IndexType type, params ColumnName[] columns) : this(type, false, columns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="columns">The columns.</param>
        public Index(IndexType type, params string[] columns) : this(type, false, columns.Select(x => new ColumnName(x)).ToArray())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index" /> class.
        /// </summary>
        /// <param name="type">The type type.</param>
        /// <param name="unique">if set to <c>true</c> [unique].</param>
        /// <param name="columns">The column names.</param>
        public Index(IndexType type, bool unique, params ColumnName[] columns)
        {
            Type = type;
            IsUnique = unique;
            Columns = columns;
        }

        /// <summary>
        /// The parent table.
        /// </summary>
        [XmlIgnore]
        public Table Table;

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        /// <value>The name.</value>
        [XmlIgnore]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) SetName();
                return _name;
            }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the index type.
        /// </summary>
        /// <value>The type.</value>
        [XmlAttribute("type"), DefaultValue(IndexType.Index)]
        public IndexType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is unique.
        /// </summary>
        /// <value><c>true</c> if the index is unique; otherwise, <c>false</c>.</value>
        [XmlAttribute("unique"), DefaultValue(false)]
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the columns associated with the index.
        /// </summary>
        /// <value>The columns referenced by the index.</value>
        [XmlElement("columnName")]
        public ColumnName[] Columns { get; set; }

        string ISchemaObject.GetName() => Name;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string parent = (string.IsNullOrEmpty(Table?.Name) ? string.Empty : $"[{Table.Name}].");
            return $"{parent}[{Name}]";
        }

        #region ICloneable

        /// <summary>
        /// Creates a new <see cref="Index"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="Index"/> object that is a copy of this instance.</returns>
        public Index Clone()
        {
            return new Index()
            {
                Columns = this.Columns,
                IsUnique = this.IsUnique,
                _name = this._name,
                Type = this.Type
            };
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

        private string _name;

        internal void Overwrite(Index right)
        {
            Type = right.Type;

            foreach (ColumnName r in right.Columns)
            {
                ColumnName? left = Find(r);
            }
        }

        internal void SetName()
        {
            string tableName = (string.IsNullOrEmpty(Table?.Name) ? string.Empty : $"{Table.Name}__");
            string columns = string.Join("_and_", Columns.Select(x => x.Name));

            _name = string.Concat(tableName, columns, "_index");
        }

        private ColumnName? Find(ColumnName right)
        {
            ColumnName left;
            for (int i = 0; i < Columns.Length; i++)
            {
                left = Columns[i];

                if (left.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase))
                {
                    left.Order = right.Order;
                    return left;
                }
            }

            return null;
        }

        private string ToDebuggerDisplay()
        {
            return $"{Name} ({string.Join(", ", Columns)})";
        }

        #endregion Private Members
    }
}