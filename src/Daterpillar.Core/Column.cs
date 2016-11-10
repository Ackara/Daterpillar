﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Represents a database column.
    /// </summary>
    public class Column : ICloneable<Column>
    {
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

        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        [XmlElement("modifier")]
        public List<string> Modifiers
        {
            get
            {
                if (_modifiers == null)
                {
                    _modifiers = new List<string>();
                }

                return _modifiers;
            }
            set { _modifiers = value; }
        }

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

        #region Private Members

        private List<string> _modifiers;

        #endregion Private Members
    }
}