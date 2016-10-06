using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public sealed class MySQLTemplate : ITemplate
    {
        public MySQLTemplate() : this(MySQLTemplateSettings.Default, new MySQLTypeNameResolver())
        {
        }

        public MySQLTemplate(bool dropDatabase) : this(MySQLTemplateSettings.Default, new MySQLTypeNameResolver())
        {
            _settings.DropDatabaseIfExist = dropDatabase;
        }

        public MySQLTemplate(ITypeNameResolver nameResolver) : this(MySQLTemplateSettings.Default, nameResolver)
        {
        }

        public MySQLTemplate(MySQLTemplateSettings settings) : this(settings, new MySQLTypeNameResolver())
        {
        }

        public MySQLTemplate(MySQLTemplateSettings settings, ITypeNameResolver nameResolver)
        {
            _settings = settings;
            _nameResolver = nameResolver;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();

            if (_settings.DropDatabaseIfExist)
            {
                _text.AppendLine($"DROP DATABASE IF EXISTS `{schema.Name}`;");
            }

            _text.AppendLine($"CREATE DATABASE IF NOT EXISTS `{schema.Name}`;");
            _text.AppendLine($"USE `{schema.Name}`;");
            _text.AppendLine();

            foreach (var table in schema.Tables)
            {
                Transform(table);
            }

            _text.AppendLine(schema.Script);
            return _text.ToString().Trim();
        }

        #region Private Members

        private int _seed = 1;
        private MySQLTemplateSettings _settings;
        private ITypeNameResolver _nameResolver;
        private StringBuilder _text = new StringBuilder();

        private void Transform(Table table)
        {
            AppendComment(table);
            _text.AppendLine($"CREATE TABLE IF NOT EXISTS `{table.Name}`");
            _text.AppendLine("(");
            foreach (var column in table.Columns)
            {
                Transform(column);
            }

            foreach (var index in table.Indexes.Where(x => x.Type == IndexType.PrimaryKey))
            {
                _text.AppendLine($"\tPRIMARY KEY ({string.Join(", ", index.Columns.Select(x => $"`{x.Name}` {x.Order}"))}),");
            }

            foreach (var foreignKey in table.ForeignKeys)
            {
                Transform(foreignKey);
            }

            _text.RemoveLastComma();
            _text.AppendLine(");");
            _text.AppendLine();

            bool hasIndex = false;
            foreach (var index in table.Indexes.Where(x => x.Type == IndexType.Index))
            {
                hasIndex = true;
                Transform(index, table.Name);
            }
            if (hasIndex) _text.AppendLine();
        }

        private void Transform(Column column)
        {
            string dataType = _nameResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL ");
            string modifiers = string.Join(" ", column.Modifiers);
            string autoIncrement = column.AutoIncrement ? " PRIMARY KEY AUTO_INCREMENT " : " ";

            _text.AppendLine($"\t`{column.Name}` {dataType}{notNull}{modifiers}{autoIncrement}COMMENT '{column.Comment?.Replace("'", "''")}',");
        }

        private void Transform(ForeignKey foreignKey)
        {
            _text.AppendLine($"\tFOREIGN KEY (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}`(`{foreignKey.ForeignColumn}`) ON UPDATE {foreignKey.OnUpdate} ON DELETE {foreignKey.OnDelete},");
        }

        private void Transform(Index index, string tableName)
        {
            string unique = index.Unique ? " UNIQUE " : " ";
            string columns = string.Join(", ", index.Columns.Select(x => $"`{x.Name}` {x.Order}"));
            index.Name = (string.IsNullOrEmpty(index.Name) ? $"{tableName}_idx{_seed++}" : index.Name);

            _text.AppendLine($"CREATE{unique}INDEX `{index.Name}` ON `{tableName}` ({columns});");
        }

        private void AppendComment(Table table)
        {
            if (_settings.CommentsEnabled)
            {
                if (string.IsNullOrWhiteSpace(table.Comment))
                {
                    _text.AppendLine("-- ------------------------------");
                    _text.AppendLine($"-- {table.Name.ToUpper()} TABLE");
                    _text.AppendLine("-- ------------------------------");
                }
                else
                {
                    _text.AppendLine("/*");
                    _text.AppendLine($"{table.Comment.AppendPeriod()}");
                    _text.AppendLine("*/");
                }
            }
        }

        #endregion Private Members
    }
}