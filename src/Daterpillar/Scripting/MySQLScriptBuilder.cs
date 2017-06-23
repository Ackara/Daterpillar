using Acklann.Daterpillar.TypeResolvers;
using System.Linq;

namespace Acklann.Daterpillar.Scripting
{
    /// <summary>
    /// Provides functions used for building MySQL scripts.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Scripting.SqlScriptBuilderBase" />
    public class MySQLScriptBuilder : SqlScriptBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLScriptBuilder"/> class.
        /// </summary>
        public MySQLScriptBuilder() : this(new SqlScriptBuilderSettings()
        {
            ShowHeader = true,
            IgnoreScripts = false,
            IgnoreComments = false,
            AppendCreateSchemaCommand = false
        })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLScriptBuilder"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MySQLScriptBuilder(SqlScriptBuilderSettings settings) : base(settings, new MySQLTypeResolver())
        {
        }

        /// <summary>
        /// Appends the MySQL representation of a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Schema schema)
        {
            if (Settings.AppendCreateSchemaCommand)
            {
                _script.AppendLine($"CREATE DATABASE IF NOT EXISTS `{schema.Name}`;");
                _script.AppendLine($"USE `{schema.Name}`;");
                _script.AppendLine();
            }

            foreach (var table in schema.Tables) Append(table);

            foreach (var script in schema.Scripts)
            {
                if (script.Syntax == Syntax.MySQL || script.Syntax == Syntax.Generic)
                {
                    Append(script);
                }
            }

            return this;
        }

        /// <summary>
        /// Appends the MySQL representation of a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Table table)
        {
            WriteComment(table);
            _script.AppendLine($"CREATE TABLE IF NOT EXISTS `{table.Name}`");
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
        /// Appends the MySQL representation of a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Column column)
        {
            string table = column.Table.Name;
            string dataType = TypeResolver.GetTypeName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");
            string comment = (string.IsNullOrEmpty(column.Comment) ? string.Empty : $" COMMENT '{column.Comment}'");

            _script.AppendLine($"ALTER TABLE `{table}` ADD `{column.Name}` {dataType}{notNull}{defaultValue}{autoIncrement}{comment};");
            return this;
        }

        /// <summary>
        /// Appends the MySQL representation of a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(Index index)
        {
            string columns = string.Join(", ", index.Columns.Select(x => ($"`{x.Name}` {x.Order.ToText()}")));

            switch (index.Type)
            {
                default:
                case IndexType.Index:
                    string unique = (index.IsUnique ? " UNIQUE " : " ");
                    _script.AppendLine($"CREATE{unique}INDEX `{index.GetName()}` ON `{index.Table.Name}` ({columns});");
                    break;

                case IndexType.PrimaryKey:
                    _script.AppendLine($"ALTER TABLE `{index.Table.Name}` ADD PRIMARY KEY ({columns});");
                    break;
            }

            return this;
        }

        /// <summary>
        /// Appends the MySQL representation of a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Append(ForeignKey foreignKey)
        {
            string name = foreignKey.GetName();
            _script.AppendLine($"ALTER TABLE `{foreignKey.LocalTable}` ADD FOREIGN KEY `{name}` (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}` (`{foreignKey.ForeignColumn}`) ON UPDATE {foreignKey.OnUpdate.ToText()} ON DELETE {foreignKey.OnDelete.ToText()};");
            return this;
        }

        /// <summary>
        /// Appends the MySQL command to remove a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Schema schema)
        {
            _script.AppendLine($"DROP DATABASE IF EXISTS `{schema.Name}`;");
            return this;
        }

        /// <summary>
        /// Appends the MySQL command to remove a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Table table)
        {
            _script.AppendLine($"DROP TABLE IF EXISTS `{table.Name}`;");
            return this;
        }

        /// <summary>
        /// Appends the MySQL command to remove a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Column column)
        {
            var table = column.Table.Name;
            var schema = column.Table.Schema;

            _script.AppendLine($"ALTER TABLE `{table}` DROP COLUMN `{column.Name}`;");
            return this;
        }

        /// <summary>
        /// Appends the MySQL command to remove a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(Index index)
        {
            _script.AppendLine($"DROP INDEX `{index.GetName()}` ON `{index.Table.Name}`;");
            return this;
        }

        /// <summary>
        /// Appends the MySQL command to remove a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Remove(ForeignKey foreignKey)
        {
            _script.AppendLine($"ALTER TABLE `{foreignKey.Table.Name}` DROP FOREIGN KEY `{foreignKey.GetName()}`;");
            return this;
        }

        /// <summary>
        /// Appends the MySQL command to update a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public override IScriptBuilder Update(Table oldTable, Table newTable)
        {
            _script.AppendLine($"ALTER TABLE `{oldTable.Name}` COMMENT '{newTable.Comment}';");
            return this;
        }

        /// <summary>
        /// Appends the MySQL command to update a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
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
            string autoIncrement = ((!oldColumn.AutoIncrement && newColumn.AutoIncrement) ? $" PRIMARY KEY AUTO_INCREMENT" : string.Empty);
            string defaultValue = (newColumn.DefaultValue == null ? string.Empty : $" DEFAULT '{newColumn.DefaultValue}'");
            string comment = (string.IsNullOrEmpty(newColumn.Comment) ? string.Empty : $" COMMENT '{newColumn.Comment}'");

            _script.AppendLine($"ALTER TABLE `{oldTable}` CHANGE COLUMN `{oldColumn.Name}` `{newColumn.Name}` {dataType}{notNull}{defaultValue}{autoIncrement}{comment};");
            return this;
        }

        #region Private Members

        private void AppendToTable(Column column)
        {
            if (column.AutoIncrement) column.IsNullable = false;

            string dataType = TypeResolver.GetTypeName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY AUTO_INCREMENT" : string.Empty);
            string comment = (string.IsNullOrEmpty(column.Comment) ? string.Empty : $" COMMENT '{column.Comment}'");
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");

            _script.AppendLine($"\t`{column.Name}` {dataType}{notNull}{defaultValue}{autoIncrement}{comment},");
        }

        private void AppendToTable(ForeignKey foreignKey)
        {
            string onUpdate = $" ON UPDATE {foreignKey.OnUpdate.ToText()}";
            string onDelete = $" ON DELETE {foreignKey.OnDelete.ToText()}";

            _script.AppendLine($"\tCONSTRAINT `{foreignKey.GetName()}` FOREIGN KEY (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}`(`{foreignKey.ForeignColumn}`){onUpdate}{onDelete},");
        }

        #endregion Private Members
    }
}