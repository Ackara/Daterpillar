using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a <see cref="Table"/> column.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    public class Column : ICloneable<Column>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        public Column() : this(null, new DataType())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="autoIncrement">if set to <c>true</c> [automatic increment].</param>
        /// <param name="nullable">if set to <c>true</c> [nullable].</param>
        /// <param name="defaultValue">The default value.</param>
        public Column(string name, DataType dataType, bool autoIncrement = false, bool nullable = false, string defaultValue = null)
        {
            Name = name;
            DataType = dataType;
            IsNullable = nullable;
            AutoIncrement = autoIncrement;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// The parent table.
        /// </summary>
        [XmlIgnore]
        public Table Table;

        /// <summary>
        /// Gets or sets the column name.
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
        /// Gets or sets the SQL data-type.
        /// </summary>
        /// <value>The type of the data.</value>
        [XmlElement("dataType")]
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value><c>true</c> if this instance is nullable; otherwise, <c>false</c>.</value>
        [XmlAttribute("nullable")]
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether column is auto incremented.
        /// </summary>
        /// <value><c>true</c> if [automatic incremented]; otherwise, <c>false</c>.</value>
        [XmlAttribute("autoIncrement")]
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        [XmlAttribute("default")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the ordinal position of the column.
        /// </summary>
        /// <value>The ordinal position.</value>
        [XmlIgnore]
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// Creates a new <see cref="Column"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="Column"/> object that is a copy of this instance.</returns>
        public Column Clone()
        {
            return new Column()
            {
                Name = this.Name,
                AutoIncrement = this.AutoIncrement,
                Comment = this.Comment,
                DataType = this.DataType,
                DefaultValue = this.DefaultValue,
                IsNullable = this.IsNullable,
                OrdinalPosition = this.OrdinalPosition
            };
        }

        internal string ToDebuggerDisplay()
        {
            return $"{Name} {DataType}{(AutoIncrement ? " autoincrement" : string.Empty)}";
        }
    }
}