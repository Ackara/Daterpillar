using Ackara.Daterpillar.TypeResolvers;
using System;
using System.Linq;

namespace Ackara.Daterpillar.Scripting
{
    public class MySQLScriptBuilder : SqlScriptBuilderBase
    {
        public MySQLScriptBuilder() : this(new SqlScriptBuilderSettings()
        {
            ShowHeader = true,
            IgnoreScripts = false,
            IgnoreComments = false,
            AppendCreateSchemaCommand = false
        })
        { }

        public MySQLScriptBuilder(SqlScriptBuilderSettings settings) : base(settings, new MySQLTypeResolver())
        {
        }

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

        public override IScriptBuilder Append(ForeignKey foreignKey)
        {
            string name = foreignKey.GetName();
            _script.AppendLine($"ALTER TABLE `{foreignKey.LocalTable}` ADD FOREIGN KEY `{name}` (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}` (`{foreignKey.ForeignColumn}`) ON UPDATE {foreignKey.OnUpdate.ToText()} ON DELETE {foreignKey.OnDelete.ToText()};");
            return this;
        }

        public override IScriptBuilder Remove(Schema schema)
        {
            _script.AppendLine($"DROP DATABASE IF EXISTS `{schema.Name}`;");
            return this;
        }

        public override IScriptBuilder Remove(Table table)
        {
            _script.AppendLine($"DROP TABLE IF EXISTS `{table.Name}`;");
            return this;
        }

        public override IScriptBuilder Remove(Column column)
        {
            var table = column.Table.Name;
            var schema = column.Table.Schema;

            _script.AppendLine($"ALTER TABLE `{table}` DROP COLUMN `{column.Name}`;");
            return this;
        }

        public override IScriptBuilder Remove(Index index)
        {
            _script.AppendLine($"DROP INDEX `{index.GetName()}` ON `{index.Table.Name}`;");
            return this;
        }

        public override IScriptBuilder Remove(ForeignKey foreignKey)
        {
            _script.AppendLine($"ALTER TABLE `{foreignKey.Table.Name}` DROP FOREIGN KEY `{foreignKey.GetName()}`;");
            return this;
        }

        public override IScriptBuilder Update(Table oldTable, Table newTable)
        {
            _script.AppendLine($"ALTER TABLE `{oldTable.Name}` COMMENT '{newTable.Comment}';");
            return this;
        }

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
            string onUpdate = (foreignKey.OnUpdate != ReferentialAction.NoAction ? $" ON UPDATE {foreignKey.OnUpdate.ToText()}" : string.Empty);
            string onDelete = (foreignKey.OnDelete != ReferentialAction.NoAction ? $" ON DELETE {foreignKey.OnDelete.ToText()}" : string.Empty);

            _script.AppendLine($"\tCONSTRAINT `{foreignKey.GetName()}` FOREIGN KEY (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}`(`{foreignKey.ForeignColumn}`){onUpdate}{onDelete},");
        }

        private void RemoveAllReferencesToColumn(Index index, string name)
        {
            throw new NotImplementedException();
        }

        private void RemoveAllReferencesToColumn(ForeignKey constraint, string table, string name)
        {
            throw new NotImplementedException();
        }

        #endregion Private Members
    }
}