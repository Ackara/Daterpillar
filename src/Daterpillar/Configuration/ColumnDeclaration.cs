﻿using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents a <see cref="Table"/> column.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public class ColumnDeclaration : ISqlStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDeclaration"/> class.
        /// </summary>
        public ColumnDeclaration() : this(null, new DataType())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDeclaration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="autoIncrement">if set to <c>true</c> [automatic increment].</param>
        /// <param name="nullable">if set to <c>true</c> [nullable].</param>
        /// <param name="defaultValue">The default value.</param>
        public ColumnDeclaration(string name, SchemaType dataType, bool autoIncrement = false, bool nullable = false, string defaultValue = null) : this(name, new DataType(dataType), autoIncrement, nullable, defaultValue)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDeclaration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="autoIncrement">if set to <c>true</c> [automatic increment].</param>
        /// <param name="nullable">if set to <c>true</c> [nullable].</param>
        /// <param name="defaultValue">The default value.</param>
        public ColumnDeclaration(string name, DataType dataType, bool autoIncrement = false, bool nullable = false, string defaultValue = null)
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
        public TableDeclaration Table;

        [XmlAttribute("suid")]
        public string Id { get; set; }

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
        [XmlIgnore]
        public int OrdinalPosition { get; set; }

        string ISqlStatement.GetName() => Name;

        internal void Overwrite(ColumnDeclaration right)
        {
            Comment = right.Comment;
            DataType = right.DataType;
            IsNullable = right.IsNullable;
            DefaultValue = right.DefaultValue;
            AutoIncrement = right.AutoIncrement;
        }

        #region ICloneable

        /// <summary>
        /// Creates a new <see cref="ColumnDeclaration"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ColumnDeclaration"/> object that is a copy of this instance.</returns>
        public ColumnDeclaration Clone()
        {
            return new ColumnDeclaration()
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

        #region Private Members

        private string GetDebuggerDisplay()
        {
            return $"{Name}: {DataType}";
        }

        #endregion Private Members
    }
}