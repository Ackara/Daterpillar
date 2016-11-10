using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public sealed class MySQLScriptBuilder : IScriptBuilder
    {
        public MySQLScriptBuilder() : this(ScriptBuilderSettings.Default, new MySQLTypeNameResolver())
        {
        }

        public MySQLScriptBuilder(ITypeNameResolver typeNameResolver) : this(ScriptBuilderSettings.Default, typeNameResolver)
        {
        }

        public MySQLScriptBuilder(ScriptBuilderSettings settings) : this(settings, new MySQLTypeNameResolver())
        {
        }

        public MySQLScriptBuilder(ScriptBuilderSettings settings, ITypeNameResolver typeNameResolver)
        {
            _settings = settings;
            _typeResolver = typeNameResolver;
        }

        public void Append(string text)
        {
            _script.Append(text);
        }

        public void AppendLine(string text)
        {
            _script.AppendLine(text);
        }

        public void Create(Schema schema)
        {
            string lineBreak = "-- ======================================================================";

            _script.AppendLine(lineBreak);
            _script.AppendLine("-- NAME:");
            _script.AppendLine($"-- {schema.Name}");
            _script.AppendLine();
            _script.AppendLine("-- DESCRIPTION:");
            _script.AppendLine($"-- {schema.Description}");
            _script.AppendLine();
            _script.AppendLine("-- AUTHOR:");
            _script.AppendLine($"-- {schema.Author}");
            _script.AppendLine();
            _script.AppendLine("-- DATE:");
            _script.AppendLine($"-- {schema.CreatedOn.ToString("ddd dd, yyyy hh:mm tt")}");
            _script.AppendLine(lineBreak);
            _script.AppendLine();

            if (_settings.TruncateDatabaseIfItExist) Drop(schema);
            if (_settings.CreateDatabase)
            {
                _script.AppendLine($"CREATE DATABASE IF NOT EXISTS `{schema.Name}`;");
                _script.AppendLine($"USE `{schema.Name}`;");
                _script.AppendLine();
            }

            foreach (var table in schema.Tables) Create(table);

            if (_settings.AppendScripts)
            {
                lineBreak = "-- =================";
                _script.AppendLine(lineBreak);
                _script.AppendLine($"-- SCRIPTS (000)");
                _script.AppendLine(lineBreak);
                _script.AppendLine();

                _script.AppendLine(schema.Script);
            }
        }

        public void Create(Table table)
        {
            _script.AppendLine($"CREATE TABLE IF NOT EXISTS `{table.Name}`");
            _script.AppendLine("(");

            foreach (var column in table.Columns) AppendToTable(column);
            foreach (var constraint in table.ForeignKeys) AppendToTable(constraint);

            _script.Remove((_script.Length - 3), 3);
            _script.AppendLine();
            _script.AppendLine(");");
            _script.AppendLine();

            foreach (var index in table.Indexes)
            {
                index.Table = table.Name;
                Create(index);
            }

            _script.AppendLine();
        }

        public void Create(Column column)
        {
            string table = column.TableRef.Name;
            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");
            string comment = (string.IsNullOrEmpty(column.Comment) ? string.Empty : $" COMMENT '{column.Comment}'");

            _script.AppendLine($"ALTER TABLE `{table}` ADD `{column.Name}` {dataType}{notNull}{defaultValue}{autoIncrement}{comment};");
        }

        public void Create(Index index)
        {
            string unique = (index.Unique ? " UNIQUE " : " ");
            string columns = string.Join(", ", index.Columns.Select(x => ($"`{x.Name}` {x.Order}")));
            
            _script.AppendLine($"CREATE{unique}INDEX `{index.GetName(_seed++)}` ON `{index.TableRef.Name}` ({columns});");
        }

        public void Create(ForeignKey foreignKey)
        {
            _script.AppendLine($"ALTER TABLE `{foreignKey.LocalTable}` ADD FOREIGN KEY `{foreignKey.Name}` (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}` (`{foreignKey.ForeignColumn}`) ON UPDATE {foreignKey.OnUpdate.ToText()} ON DELETE {foreignKey.OnDelete.ToText()};");
        }

        public void Drop(Schema schema)
        {
            _script.AppendLine($"DROP DATABASE IF EXISTS `{schema.Name}`;");
        }

        public void Drop(Table table)
        {
            _script.AppendLine($"DROP TABLE IF EXISTS `{table.Name}`;");
        }

        public void Drop(Column column)
        {
            var table = column.TableRef.Name;
            var schema = column.TableRef.SchemaRef;

            foreach (var constraint in schema.GetForeignKeys()) RemoveAllReferencesToColumn(constraint, table, column.Name);
            foreach (var index in schema.GetIndexes()) RemoveAllReferencesToColumn(index, column.Name);

            _script.AppendLine($"ALTER TABLE `{table}` DROP COLUMN `{column.Name}`;");
        }

        public void Drop(Index index)
        {
            _script.AppendLine($"DROP INDEX `{index.Name}` ON `{index.TableRef.Name}`;");
        }

        public void Drop(ForeignKey foreignKey)
        {
            _script.AppendLine($"ALTER TABLE `{foreignKey.TableRef.Name}` DROP FOREIGN KEY `{foreignKey.Name}`;");
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
            _script.AppendLine($"ALTER TABLE `{oldTable.Name}` COMMENT '{newTable.Comment}';");
        }

        public void AlterTable(Column oldColumn, Column newColumn)
        {
            if (newColumn.AutoIncrement) newColumn.IsNullable = false;

            string oldTable = oldColumn.TableRef.Name;
            bool renameRequired = oldColumn.Name != newColumn.Name;
            string dataType = _typeResolver.GetName(newColumn.DataType);
            string notNull = (newColumn.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (newColumn.AutoIncrement ? $" PRIMARY KEY AUTO_INCREMENT" : string.Empty);
            string defaultValue = (newColumn.DefaultValue == null ? string.Empty : $" DEFAULT '{newColumn.DefaultValue}'");
            string comment = (string.IsNullOrEmpty(newColumn.Comment) ? string.Empty : $" COMMENT '{newColumn.Comment}'");

            _script.AppendLine($"ALTER TABLE `{oldTable}` CHANGE COLUMN `{oldColumn.Name}` `{newColumn.Name}` {dataType}{notNull}{defaultValue}{autoIncrement}{comment};");
        }

        public string GetContent()
        {
            return _script.ToString();
        }

        public void Clear()
        {
            _script.Clear();
        }

        #region Private Members

        private readonly ITypeNameResolver _typeResolver;
        private readonly ScriptBuilderSettings _settings;
        private readonly StringBuilder _script = new StringBuilder();

        private int _seed = 1;

        private void AppendToTable(Column column)
        {
            if (column.AutoIncrement) column.IsNullable = false;

            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY AUTO_INCREMENT" : string.Empty);
            string comment = (string.IsNullOrEmpty(column.Comment) ? string.Empty : $" COMMENT '{column.Comment}'");
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");

            _script.AppendLine($"\t`{column.Name}` {dataType}{notNull}{defaultValue}{autoIncrement}{comment},");
        }

        private void AppendToTable(ForeignKey foreignKey)
        {
            string onUpdate = (foreignKey.OnUpdate != ForeignKeyRule.RESTRICT ? $" ON UPDATE {foreignKey.OnUpdate}" : string.Empty);
            string onDelete = (foreignKey.OnDelete != ForeignKeyRule.RESTRICT ? $" ON DELETE {foreignKey.OnDelete}" : string.Empty);
            
            _script.AppendLine($"\tCONSTRAINT `{foreignKey.GetName(_seed++)}` FOREIGN KEY (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}`(`{foreignKey.ForeignColumn}`){onUpdate}{onDelete},");
        }

        private void RemoveAllReferencesToColumn(Index index, string columnName)
        {
            bool indexColumnsWereRemoved = (index.Columns.RemoveAll(x => x.Name == columnName) > 0);
            if (indexColumnsWereRemoved)
            {
                bool shouldRemoveIndex = (index.Columns.Count == 0);

                if (shouldRemoveIndex)
                {
                    Drop(index);
                }
                else /* All of the index columns were NOT removed */
                {
                    Drop(index);
                    Create(index);
                }
            }
        }

        private void RemoveAllReferencesToColumn(ForeignKey constraint, string tableName, string columnName)
        {
            if ((constraint.LocalTable == tableName && constraint.LocalColumn == columnName)
                || constraint.ForeignTable == tableName && constraint.ForeignColumn == columnName)
            {
                Drop(constraint);
            }
        }

        #endregion Private Members
    }
}