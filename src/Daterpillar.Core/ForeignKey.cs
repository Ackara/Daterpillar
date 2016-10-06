using System.Xml.Serialization;

namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Represents a database foreign key.
    /// </summary>
    public class ForeignKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey" /> class.
        /// </summary>
        public ForeignKey()
        {
            OnUpdate = ForeignKeyRule.CASCADE;
            OnDelete = ForeignKeyRule.CASCADE;
        }

        public Table TableRef;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the table this constraint belongs to.
        /// </summary>
        /// <value>The local table.</value>
        [XmlAttribute("table")]
        public string LocalTable
        {
            get
            {
                if (TableRef == null) return string.Empty;
                else return TableRef.Name;
            }
            set
            {
                if (TableRef == null)
                {
                    TableRef = new Table();
                }

                TableRef.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the local column.
        /// </summary>
        /// <value>The local column.</value>
        [XmlAttribute("localColumn")]
        public string LocalColumn { get; set; }

        /// <summary>
        /// Gets or sets the foreign table.
        /// </summary>
        /// <value>The foreign table.</value>
        [XmlAttribute("foreignTable")]
        public string ForeignTable { get; set; }

        /// <summary>
        /// Gets or sets the foreign column.
        /// </summary>
        /// <value>The foreign column.</value>
        [XmlAttribute("foreignColumn")]
        public string ForeignColumn { get; set; }

        /// <summary>
        /// Gets or sets the on update rule.
        /// </summary>
        /// <value>The on update rule.</value>
        [XmlAttribute("onUpdate")]
        public ForeignKeyRule OnUpdate { get; set; }

        /// <summary>
        /// Gets or sets the on delete rule.
        /// </summary>
        /// <value>The on delete rule.</value>
        [XmlAttribute("onDelete")]
        public ForeignKeyRule OnDelete { get; set; }
    }
}