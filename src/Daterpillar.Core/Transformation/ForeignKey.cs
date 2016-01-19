using System;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    /// <summary>
    /// Represents a database foreign key.
    /// </summary>
    public class ForeignKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class.
        /// </summary>
        public ForeignKey()
        {
            OnUpdateRule = ForeignKeyRule.CASCADE;
            OnDeleteRule = ForeignKeyRule.CASCADE;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the local column.
        /// </summary>
        /// <value>
        /// The local column.
        /// </value>
        [XmlAttribute("localColumn")]
        public string LocalColumn { get; set; }

        /// <summary>
        /// Gets or sets the foreign table.
        /// </summary>
        /// <value>
        /// The foreign table.
        /// </value>
        [XmlAttribute("foreignTable")]
        public string ForeignTable { get; set; }

        /// <summary>
        /// Gets or sets the foreign column.
        /// </summary>
        /// <value>
        /// The foreign column.
        /// </value>
        [XmlAttribute("foreignColumn")]
        public string ForeignColumn { get; set; }

        /// <summary>
        /// Gets or sets the on update rule.
        /// </summary>
        /// <value>
        /// The on update.
        /// </value>
        [XmlElement("onUpdate")]
        public string OnUpdate { get; set; }

        /// <summary>
        /// Gets or sets the on delete.
        /// </summary>
        /// <value>
        /// The on delete.
        /// </value>
        [XmlElement("onDelete")]
        public string OnDelete { get; set; }

        /// <summary>
        /// Gets or sets the on update rule.
        /// </summary>
        /// <value>
        /// The on update rule.
        /// </value>
        [XmlIgnore]
        public ForeignKeyRule OnUpdateRule
        {
            get { return (ForeignKeyRule)Enum.Parse(typeof(ForeignKeyRule), OnUpdate.Replace(" ", "_")); }
            set { OnUpdate = value.ToText(); }
        }

        /// <summary>
        /// Gets or sets the on delete rule.
        /// </summary>
        /// <value>
        /// The on delete rule.
        /// </value>
        [XmlIgnore]
        public ForeignKeyRule OnDeleteRule
        {
            get { return (ForeignKeyRule)Enum.Parse(typeof(ForeignKeyRule), OnDelete.Replace(" ", "_")); }
            set { OnDelete = value.ToText(); }
        }
    }
}