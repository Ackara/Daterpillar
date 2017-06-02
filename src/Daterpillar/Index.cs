using System.Linq;
using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a <see cref="Table"/> index.
    /// </summary>
    public class Index : ICloneable<Index>
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
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the index type.
        /// </summary>
        /// <value>The type.</value>
        [XmlAttribute("type")]
        public IndexType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is unique.
        /// </summary>
        /// <value><c>true</c> if the index is unique; otherwise, <c>false</c>.</value>
        [XmlAttribute("unique")]
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the columns associated with the index.
        /// </summary>
        /// <value>The columns referenced by the index.</value>
        [XmlElement("columnName")]
        public ColumnName[] Columns { get; set; }

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
                Name = this.Name,
                Type = this.Type
            };
        }

        internal string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                string tableName = (string.IsNullOrEmpty(Table?.Name) ? string.Empty : $"{Table.Name}_");
                string columns = string.Join("_and_", Columns.Select(x => x.Name));

                return string.Concat(tableName, columns);
            }
            else return Name;
        }
    }
}