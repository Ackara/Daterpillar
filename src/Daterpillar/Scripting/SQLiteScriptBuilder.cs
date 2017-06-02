using Acklann.Daterpillar.TypeResolvers;
using System.Linq;

namespace Acklann.Daterpillar.Scripting
{
    /// <summary>
    /// Provides functions used for building SQLite scripts.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Scripting.SqlScriptBuilderBase" />
    public class SQLiteScriptBuilder : SqlScriptBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteScriptBuilder"/> class.
        /// </summary>
        public SQLiteScriptBuilder() : base(new SqlScriptBuilderSettings()
        {
            ShowHeader = true,
            IgnoreScripts = false,
            IgnoreComments = false,
            AppendCreateSchemaCommand = false
        }, new SQLiteTypeResolver())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteScriptBuilder"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SQLiteScriptBuilder(SqlScriptBuilderSettings settings) : base(settings, new SQLiteTypeResolver())
        {
        }

        /// <summary>
        /// Appends the SQLite representation of a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Schema schema)
        {
            foreach (var table in schema.Tables) Append(table);

            foreach (var script in schema.Scripts)
            {
                if (script.Syntax == Syntax.SQLite || script.Syntax == Syntax.Generic)
                {
                    Append(script);
                }
            }

            return this;
        }

        /// <summary>
        /// Appends the SQLite representation of a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Table table)
        {
            WriteComment(table);
            _script.AppendLine($"CREATE TABLE IF NOT EXISTS [{table.Name}]");
            _script.AppendLine("(");

            foreach (var column in table.Columns) AppendToTable(column);
            foreach (var constraint in table.ForeignKeys) AppendToTable(constraint);

            _script.Remove((_script.Length - 3), 3);
            _script.AppendLine();
            _script.AppendLine(");");
            _script.AppendLine();

            foreach (var index in table.Indexes) Append(index);

            _script.AppendLine();
            return this;
        }

        /// <summary>
        /// Appends the SQLite representation of a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Column column)
        {
            string table = column.Table.Name;
            string dataType = TypeResolver.GetTypeName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string defaultValue = (!column.IsNullable ? $" DEFAULT '{column.DefaultValue ?? string.Empty}'" : string.Empty);
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY AUTOINCREMENT" : string.Empty);

            _script.AppendLine($"ALTER TABLE [{table}] ADD COLUMN [{column.Name}] {dataType}{notNull}{defaultValue}{autoIncrement};");
            return this;
        }

        /// <summary>
        /// Appends the SQLite representation of a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Index index)
        {
            string table = index.Table.Name;
            string unique = (index.IsUnique ? " UNIQUE " : " ");
            string columns = string.Join(", ", index.Columns.Select(x => ($"[{x.Name}] {x.Order.ToText()}")));

            _script.AppendLine($"CREATE{unique}INDEX IF NOT EXISTS [{index.GetName()}] ON [{index.Table.Name}] ({columns});");
            return this;
        }

        /// <summary>
        /// Appends the SQLite representation of a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(ForeignKey foreignKey)
        {
            string table = foreignKey.LocalTable;
            string tempName = $"{table}_temp_table";

            var columns = string.Join(", ", foreignKey.Table.Columns.Select(x => $"[{x.Name}]"));
            var clone = foreignKey.Table.Clone();
            clone.ForeignKeys.Add(foreignKey);

            _script.AppendLine($"/* ADD ([{table}].[{foreignKey.GetName()}]) SCRIPT */");
            _script.AppendLine("PRAGMA foreign_keys = 0;");
            _script.AppendLine($"CREATE TABLE [{tempName}] AS SELECT * FROM [{table}];");
            _script.AppendLine($"DROP TABLE [{table}];");
            _script.AppendLine();

            Append(clone);

            _script.AppendLine($"INSERT INTO [{table}] ({columns}) SELECT {columns} FROM [{tempName}];");
            _script.AppendLine($"DROP TABLE [{tempName}];");
            _script.AppendLine("PRAGMA foreign_keys = 1;");
            _script.AppendLine("/* END SCRIPT */");
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Schema schema)
        {
            _script.AppendLine("/* SQLite do not support DROP DATABASE */");
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Table table)
        {
            _script.AppendLine($"DROP TABLE IF EXISTS [{table.Name}];");
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Column column)
        {
            Table table = column.Table;
            string tempName = $"{table.Name}_temp_table";
            var copyOfTable = column.Table.Clone();
            copyOfTable.Columns.RemoveAll(col => col.Name == column.Name);
            var columns = string.Join(", ", copyOfTable.Columns.Select(x => $"[{x.Name}]"));

            _script.AppendLine($"/* DROP ([{table.Name}].[{column.Name}]) SCRIPT */");
            _script.AppendLine("PRAGMA foreign_keys = 0;");
            _script.AppendLine($"CREATE TABLE [{tempName}] AS SELECT * FROM [{table.Name}];");
            _script.AppendLine($"DROP TABLE [{table.Name}];");

            Append(copyOfTable);

            _script.AppendLine($"INSERT INTO [{table.Name}] ({columns}) SELECT {columns} FROM [{tempName}];");
            _script.AppendLine($"DROP TABLE [{tempName}];");
            _script.AppendLine("PRAGMA foreign_keys = 1;");
            _script.AppendLine("/* END SCRIPT */");
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Index index)
        {
            string table = index.Table.Name;
            _script.AppendLine($"DROP INDEX IF EXISTS [{index.GetName()}];");
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(ForeignKey foreignKey)
        {
            string table = foreignKey.LocalTable;
            string tempName = $"{table}_temp_table";
            var clonedTable = foreignKey.Table.Clone();
            clonedTable.ForeignKeys.RemoveAll(col => col.GetName() == foreignKey.GetName());
            var columns = string.Join(", ", foreignKey.Table.Columns.Select(x => $"[{x.Name}]"));

            _script.AppendLine($"/* DROP ([{table}].[{foreignKey.GetName()}]) SCRIPT */");
            _script.AppendLine("PRAGMA foreign_keys = 0;");
            _script.AppendLine($"CREATE TABLE [{tempName}] AS SELECT * FROM [{table}];");
            _script.AppendLine($"DROP TABLE [{table}];");

            Append(clonedTable);

            _script.AppendLine($"INSERT INTO [{table}] ({columns}) SELECT {columns} FROM [{tempName}];");
            _script.AppendLine($"DROP TABLE [{tempName}];");
            _script.AppendLine("PRAGMA foreign_keys = 1;");
            _script.AppendLine("/* END SCRIPT */");
            return this;
        }

        /// <summary>
        /// Appends the command to update a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Update(Column oldColumn, Column newColumn)
        {
            Table table = oldColumn.Table;
            Table clonedTable = newColumn.Table;
            string tempTable = $"{table.Name}_temp_table";

            string oldColumns = string.Join(", ", table.Columns.Select(x => $"[{x.Name}]"));
            string newColumns = string.Join(", ", clonedTable.Columns.Select(x => $"[{x.Name}]"));

            _script.AppendLine("PRAGMA foreign_keys = 0;");
            _script.AppendLine($"CREATE TABLE [{tempTable}] AS SELECT * FROM [{table.Name}];");
            _script.AppendLine($"DROP TABLE [{table.Name}];");

            Append(clonedTable);

            _script.AppendLine($"INSERT INTO [{table.Name}] ({newColumns}) SELECT {oldColumns} FROM [{tempTable}];");
            _script.AppendLine($"DROP TABLE [{tempTable}];");

            _script.AppendLine("PRAGMA foreign_keys = 1;");
            return this;
        }

        /// <summary>
        /// Appends the command to update a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Update(Table oldTable, Table newTable)
        {
            return this;
        }

        #region Private Members

        private void AppendToTable(Column column)
        {
            string dataType = TypeResolver.GetTypeName(column.DataType);
            string notNull = ((column.IsNullable && !column.AutoIncrement) ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY AUTOINCREMENT" : string.Empty);
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");

            _script.AppendLine($"\t[{column.Name}] {dataType}{notNull}{defaultValue}{autoIncrement},");
        }

        private void AppendToTable(ForeignKey foreignKey)
        {
            string onUpdate = (foreignKey.OnUpdate == ReferentialAction.NoAction ? string.Empty : $" ON UPDATE {foreignKey.OnUpdate.ToText()}");
            string onDelete = (foreignKey.OnDelete == ReferentialAction.NoAction ? string.Empty : $" ON DELETE {foreignKey.OnDelete.ToText()}");

            _script.AppendLine($"\tCONSTRAINT [{foreignKey.GetName()}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}]([{foreignKey.ForeignColumn}]){onUpdate}{onDelete},");
        }

        #endregion Private Members
    }
}