using Acklann.Daterpillar.Annotations;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Serialization
{
    /// <summary>
    /// Represents a <see cref="Table"/> foreign key.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay(), nq}")]
    public class ForeignKey : ISchemaObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class.
        /// </summary>
        public ForeignKey() : this(null, null, "Id", ReferentialAction.Cascade, ReferentialAction.Restrict)
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
        public ForeignKey(string localColumn, string foreignTable, string foreignColumn, ReferentialAction onUpdate = ReferentialAction.Cascade, ReferentialAction onDelete = ReferentialAction.Restrict)
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
        [DefaultValue(ReferentialAction.Cascade)]
        public ReferentialAction OnUpdate { get; set; }

        /// <summary>
        /// Gets or sets the 'on delete' referential action.
        /// </summary>
        /// <value>The on delete.</value>
        [XmlAttribute("onDelete")]
        [DefaultValue(ReferentialAction.Restrict)]
        public ReferentialAction OnDelete { get; set; }

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
                OnDelete = this.OnDelete,
                OnUpdate = this.OnUpdate,
                _name = this._name
            };
        }

        /// <summary>
        /// Creates a new <see cref="ForeignKey"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object System.ICloneable.Clone() => Clone();

        #endregion ICloneable

        #region Private Members

        private string _name;

        internal void SetName()
        {
            string table = string.IsNullOrEmpty(LocalTable) ? string.Empty : $"{LocalTable}_";
            _name = $"{table}{LocalColumn}_TO_{ForeignTable}_{ForeignColumn}__fk";
        }

        internal void Overwrite(ForeignKey right)
        {
            OnUpdate = right.OnUpdate;
            OnDelete = right.OnDelete;
        }

        private string ToDebuggerDisplay()
        {
            return $"[{LocalColumn}] POINTS-TO [{ForeignTable}].[{ForeignColumn}]";
        }

        #endregion Private Members
    }
}