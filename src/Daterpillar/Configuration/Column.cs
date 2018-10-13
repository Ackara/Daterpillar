using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents a <see cref="Table"/> column.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    public class Column : ISQLObject
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
        public Column(string name, SchemaType dataType, bool autoIncrement = false, bool nullable = false, string defaultValue = null) : this(name, new DataType(dataType), autoIncrement, nullable, defaultValue)
        { }

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
        [XmlIgnore, IgnoreDataMember]
        public Table Table;

        [XmlAttribute("suid"), DefaultValue(0)]
        public int Id { get; set; }

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
        [XmlElement("documentation")]
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
        [XmlAttribute("nullable"), DefaultValue(false)]
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether column is auto incremented.
        /// </summary>
        /// <value><c>true</c> if [automatic incremented]; otherwise, <c>false</c>.</value>
        [XmlAttribute("autoIncrement"), DefaultValue(false)]
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
        [XmlIgnore, IgnoreDataMember]
        public int OrdinalPosition { get; set; }

        string ISQLObject.GetName() => Name;

        #region ICloneable

        /// <summary>
        /// Creates a new <see cref="Column"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="Column"/> object that is a copy of this instance.</returns>
        public Column Clone()
        {
            return new Column()
            {
                Id = this.Id,
                Name = this.Name,
                AutoIncrement = this.AutoIncrement,
                Comment = this.Comment,
                DataType = this.DataType,
                DefaultValue = this.DefaultValue,
                IsNullable = this.IsNullable,
                OrdinalPosition = this.OrdinalPosition
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

        internal void Overwrite(Column right)
        {
            Comment = right.Comment;
            DataType = right.DataType;
            IsNullable = right.IsNullable;
            DefaultValue = right.DefaultValue;
            AutoIncrement = right.AutoIncrement;
        }

        #region Private Members

        private string ToDebuggerDisplay()
        {
            return $"{Name} {DataType}{(AutoIncrement ? " autoincrement" : string.Empty)}";
        }

        #endregion Private Members
    }
}