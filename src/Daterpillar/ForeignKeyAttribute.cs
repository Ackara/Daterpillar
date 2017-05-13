﻿using System;

namespace Ackara.Daterpillar
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
        /// <param name="foreignTable">The foreign table.</param>
        /// <param name="foreignColumn">The foreign column.</param>
        ///
        public ForeignKeyAttribute(string foreignTable, string foreignColumn)
        {
            ForeignColumn = foreignColumn;
            ForeignTable = foreignTable;
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
        public ReferentialAction OnUpdate;

        /// <summary>
        /// The on delete action.
        /// </summary>
        public ReferentialAction OnDelete;
    }
}