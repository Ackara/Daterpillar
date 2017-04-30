using Ackara.Daterpillar.TypeResolvers;
using System.Linq;

namespace Ackara.Daterpillar.Scripting
{
    /// <summary>
    /// Provides functions used for building MSSQL scripts.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Scripting.SqlScriptBuilderBase" />
    public class MSSQLScriptBuilder : SqlScriptBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSQLScriptBuilder"/> class.
        /// </summary>
        public MSSQLScriptBuilder() : this(new SqlScriptBuilderSettings()
        {
            AppendCreateSchemaCommand = false,
            IgnoreComments = false,
            IgnoreScripts = false,
            ShowHeader = true
        })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSSQLScriptBuilder"/> class.
        /// </summary>
        /// <param name="settings">The settings object.</param>
        public MSSQLScriptBuilder(SqlScriptBuilderSettings settings) : base(settings, new MSSQLTypeResolver())
        {
        }

        /// <summary>
        /// Appends the MSSQL representation of a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Schema schema)
        {
            if (Settings.AppendCreateSchemaCommand)
            {
                _script.AppendLine($"IF DB_ID('{schema.Name}') IS NULL CREATE DATABASE [{schema.Name}];");
                _script.AppendLine();
            }

            foreach (var table in schema.Tables) Append(table);

            if (Settings.IgnoreScripts == false)
            {
                foreach (var script in schema.Scripts)
                {
                    if (script.Syntax == Syntax.MSSQL || script.Syntax == Syntax.Generic)
                    {
                        Append(script);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Appends the MSSQL representation of a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Table table)
        {
            WriteComment(table);
            _script.AppendLine($"CREATE TABLE [{table.Name}]");
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
        /// Appends the MSSQL representation of a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Column column)
        {
            string table = column.Table.Name;
            string dataType = TypeResolver.GetTypeName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);

            _script.AppendLine($"ALTER TABLE [{table}] ADD [{column.Name}] {dataType}{notNull}{defaultValue}{autoIncrement};");
            return this;
        }

        /// <summary>
        /// Appends the MSSQL representation of a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Index index)
        {
            string indexName = index.GetName();
            string columns = string.Join(", ", index.Columns.Select(x => ($"[{x.Name}] {x.Order.ToText()}")));

            if (index.Type == IndexType.PrimaryKey)
            {
                _script.AppendLine($"ALTER TABLE [{index.Table.Name}] ADD PRIMARY KEY ({columns});");
            }
            else
            {
                string unique = (index.IsUnique ? " UNIQUE " : " ");
                _script.AppendLine($"CREATE{unique}INDEX [{indexName}] ON [{index.Table.Name}] ({columns});");
            }

            return this;
        }

        /// <summary>
        /// Appends the MSSQL representation of a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(ForeignKey foreignKey)
        {
            string table = foreignKey.LocalTable;
            string name = foreignKey.GetName();

            _script.AppendLine($"ALTER TABLE [{table}] WITH CHECK ADD CONSTRAINT [{name}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate.ToText()} ON DELETE {foreignKey.OnDelete.ToText()};");
            return this;
        }

        /// <summary>
        /// Appends the MSSQL command to remove a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Schema schema)
        {
            _script.AppendLine($"IF DB_ID('{schema.Name}') IS NOT NULL ALTER DATABASE [{schema.Name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
            _script.AppendLine($"IF DB_ID('{schema.Name}') IS NOT NULL DROP DATABASE [{schema.Name}];");

            return this;
        }

        /// <summary>
        /// Appends the MSSQL command to remove a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Table table)
        {
            _script.AppendLine($"DROP TABLE [{table.Name}];");
            return this;
        }

        /// <summary>
        /// Appends the MSSQL command to remove a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Column column)
        {
            var table = column.Table.Name;
            var schema = column.Table.Schema;

            _script.AppendLine($"ALTER TABLE [{table}] DROP COLUMN [{column.Name}];");

            return this;
        }

        /// <summary>
        /// Appends the MSSQL command to remove a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Index index)
        {
            string table = index.Table.Name;
            string name = (string.IsNullOrEmpty(index.Name) ? index.GetName() : index.Name);
            _script.AppendLine($"DROP INDEX [{name}] ON [{table}];");
            return this;
        }

        /// <summary>
        /// Appends the MSSQL command to remove a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(ForeignKey foreignKey)
        {
            string name = (string.IsNullOrEmpty(foreignKey.Name) ? foreignKey.GetName() : foreignKey.Name);
            _script.AppendLine($"ALTER TABLE [{foreignKey.LocalTable}] DROP CONSTRAINT [{name}];");
            return this;
        }

        /// <summary>
        /// Appends the MSSQL command to update a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Update(Table oldTable, Table newTable)
        {
            // Do nothing
            return this;
        }

        /// <summary>
        /// Appends the MSSQL command to update a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Update(Column oldColumn, Column newColumn)
        {
            if (newColumn.AutoIncrement) newColumn.IsNullable = false;

            string oldTable = oldColumn.Table.Name;
            bool renameRequired = oldColumn.Name != newColumn.Name;
            string dataType = TypeResolver.GetTypeName(newColumn.DataType);
            string notNull = (newColumn.IsNullable ? string.Empty : " NOT NULL");
            string defaultValue = (newColumn.DefaultValue == null ? string.Empty : $" DEFAULT '{newColumn.DefaultValue}'");
            string autoIncrement = ((!oldColumn.AutoIncrement && newColumn.AutoIncrement) ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);

            _script.AppendLine($"ALTER TABLE [{oldTable}] ALTER COLUMN [{oldColumn.Name}] {dataType}{notNull}{defaultValue}{autoIncrement};");
            if (renameRequired) _script.AppendLine($"EXEC sp_rename '{oldTable}.{oldColumn.Name}', '{newColumn.Name}', 'COLUMN';");

            return this;
        }

        #region Private Members

        private void AppendToTable(Column column)
        {
            string dataType = TypeResolver.GetTypeName(column.DataType);
            string notNull = (column.IsNullable && column.AutoIncrement == false) ? string.Empty : " NOT NULL";
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");

            _script.AppendLine($"\t[{column.Name}] {dataType}{notNull}{autoIncrement}{defaultValue},");
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