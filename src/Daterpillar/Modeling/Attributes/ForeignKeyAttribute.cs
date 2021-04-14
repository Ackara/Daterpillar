using System;

namespace Acklann.Daterpillar.Modeling.Attributes
{
    /// <summary>
    /// Indicates that a public field or property represents a SQL foreign key constraint. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage((AttributeTargets.Property), AllowMultiple = false, Inherited = true)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyAttribute"/> class.
        /// </summary>
        /// <param name="foreignType">The foreign table.</param>
        /// <param name="memberName">Name of the member column.</param>
        public ForeignKeyAttribute(Type foreignType, string memberName = "Id", ReferentialAction onUpdate = ReferentialAction.Restrict, ReferentialAction onDelete = ReferentialAction.Restrict) : this(foreignType.AssemblyQualifiedName, memberName, onUpdate, onDelete)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyAttribute" /> class.
        /// </summary>
        /// <param name="foreignTable">The foreign table.</param>
        /// <param name="foreignColumn">The foreign column.</param>
        /// <param name="onUpdate">The on update.</param>
        /// <param name="onDelete">The on delete.</param>
        public ForeignKeyAttribute(string foreignTable, string foreignColumn = "Id", ReferentialAction onUpdate = ReferentialAction.Restrict, ReferentialAction onDelete = ReferentialAction.Restrict)
        {
            ForeignColumn = foreignColumn;
            ForeignTable = foreignTable;
            OnUpdate = onUpdate;
            OnDelete = onDelete;
        }

        /// <summary>
        /// The referenced table.
        /// </summary>
        public readonly string ForeignTable;

        /// <summary>
        /// The referenced table column.
        /// </summary>
        public readonly string ForeignColumn;

        /// <summary>
        /// The on update action.
        /// </summary>
        public readonly ReferentialAction OnUpdate;

        /// <summary>
        /// The on delete action.
        /// </summary>
        public readonly ReferentialAction OnDelete;
    }
}