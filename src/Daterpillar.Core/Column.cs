using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a database column.
    /// </summary>
    public class Column : ICloneable<Column>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        public Column()
        {
            Comment = string.Empty;
            DefaultValue = null;
        }

        [XmlIgnore]
        public Table TableRef;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        [XmlElement("dataType")]
        public DataType DataType { get; set; }

        [XmlAttribute("nullable")]
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is auto incremented.
        /// </summary>
        /// <value><c>true</c> if [automatic increment]; otherwise, <c>false</c>.</value>
        [XmlAttribute("autoIncrement")]
        public bool AutoIncrement { get; set; }

        [XmlElement("default")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlIgnore]
        public int OrdinalPosition { get; set; }

        public Column Clone()
        {
            return new Column()
            {
                Name = this.Name,
                Comment = this.Comment,
                DataType = this.DataType,
                IsNullable = this.IsNullable,
                AutoIncrement = this.AutoIncrement,
                DefaultValue = this.DefaultValue,
                OrdinalPosition = this.OrdinalPosition
            };
        }
    }
}