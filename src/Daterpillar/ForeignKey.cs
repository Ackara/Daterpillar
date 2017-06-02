using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a <see cref="Table"/> foreign key.
    /// </summary>
    public class ForeignKey : ICloneable<ForeignKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class.
        /// </summary>
        public ForeignKey() : this(string.Empty, string.Empty, string.Empty, ReferentialAction.NoAction, ReferentialAction.NoAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class.
        /// </summary>
        /// <param name="localColumn">The local column name.</param>
        /// <param name="foreignTable">The foreign table name.</param>
        /// <param name="foreignColumn">The foreign column name.</param>
        /// <param name="onUpdate">The on update referential action.</param>
        /// <param name="onDelete">The on delete referential action.</param>
        public ForeignKey(string localColumn, string foreignTable, string foreignColumn, ReferentialAction onUpdate = ReferentialAction.NoAction, ReferentialAction onDelete = ReferentialAction.NoAction)
        {
            LocalColumn = localColumn;
            ForeignTable = foreignTable;
            ForeignColumn = foreignColumn;
            OnUpdate = onUpdate;
            OnDelete = onDelete;
        }

        /// <summary>
        /// The parent table.
        /// </summary>
        [XmlIgnore]
        public Table Table;

        /// <summary>
        /// Gets or sets the name of the key.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the local table name.
        /// </summary>
        /// <value>The local table.</value>
        [XmlIgnore]
        public string LocalTable
        {
            get { return (Table == null) ? string.Empty : Table.Name; }
            set
            {
                if (Table == null) Table = new Table();
                Table.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the local column name.
        /// </summary>
        /// <value>The local column.</value>

        [XmlAttribute("localColumn")]
        public string LocalColumn { get; set; }

        /// <summary>
        /// Gets or sets the foreign table name.
        /// </summary>
        /// <value>The foreign table.</value>
        [XmlAttribute("foreignTable")]
        public string ForeignTable { get; set; }

        /// <summary>
        /// Gets or sets the foreign column name.
        /// </summary>
        /// <value>The foreign column.</value>
        [XmlAttribute("foreignColumn")]
        public string ForeignColumn { get; set; }

        /// <summary>
        /// Gets or sets the 'on update' referential action.
        /// </summary>
        /// <value>The on update.</value>
        [XmlAttribute("onUpdate")]
        public ReferentialAction OnUpdate { get; set; }

        /// <summary>
        /// Gets or sets the 'on delete' referential action.
        /// </summary>
        /// <value>The on delete.</value>
        [XmlAttribute("onDelete")]
        public ReferentialAction OnDelete { get; set; }

        /// <summary>
        /// Creates a new <see cref="ForeignKey"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ForeignKey"/> object that is a copy of this instance.</returns>
        public ForeignKey Clone()
        {
            return new ForeignKey()
            {
                ForeignColumn = this.ForeignColumn,
                ForeignTable = this.ForeignTable,
                LocalColumn = this.LocalColumn,
                LocalTable = this.LocalTable,
                Name = this.Name,
                OnDelete = this.OnDelete,
                OnUpdate = this.OnUpdate
            };
        }

        internal string GetName()
        {
            string table = string.IsNullOrEmpty(LocalTable) ? string.Empty : $"{LocalTable}_";
            return (string.IsNullOrWhiteSpace(Name)? $"{table}{LocalColumn}_TO_{ForeignTable}_{ForeignColumn}" : Name);
        }
    }
}