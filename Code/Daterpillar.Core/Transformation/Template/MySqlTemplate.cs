using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.Transformation.Template
{
    public sealed class MySqlTemplate : ITemplate
    {
        public MySqlTemplate() : this(MySqlTemplateSettings.Default, new MySqlTypeNameResolver())
        {
        }

        public MySqlTemplate(ITypeNameResolver nameResolver) : this(MySqlTemplateSettings.Default, nameResolver)
        {
        }

        public MySqlTemplate(MySqlTemplateSettings settings, ITypeNameResolver nameResolver)
        {
            _settings = settings;
            _nameResolver = nameResolver;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();

            if (_settings.DropSchemaAtBegining)
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

            return _text.ToString().Trim();
        }

        #region Private Members

        private MySqlTemplateSettings _settings;
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

            foreach (var index in table.Indexes.Where(x => x.IndexType == IndexType.Primary))
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
            foreach (var index in table.Indexes.Where(x => x.IndexType == IndexType.Index))
            {
                hasIndex = true;
                Transform(index);
            }
            if (hasIndex) _text.AppendLine();
        }

        private void Transform(Column column)
        {
            string dataType = _nameResolver.GetName(column.DataType);
            string modifiers = string.Join(" ", column.Modifiers);
            string autoIncrement = column.AutoIncrement ? " AUTO_INCREMENT " : " ";

            _text.AppendLine($"\t`{column.Name}` {dataType} {modifiers}{autoIncrement}COMMENT '{column.Comment?.Replace("'", "''")}',");
        }

        private void Transform(ForeignKey foreignKey)
        {
            _text.AppendLine($"\tFOREIGN KEY (`{foreignKey.LocalColumn}`) REFERENCES `{foreignKey.ForeignTable}`(`{foreignKey.ForeignColumn}`) ON UPDATE {foreignKey.OnUpdate} ON DELETE {foreignKey.OnDelete},");
        }

        private void Transform(Index index)
        {
            string unique = index.Unique ? " UNIQUE " : " ";
            string columns = string.Join(", ", index.Columns.Select(x => $"`{x.Name}` {x.Order}"));

            _text.AppendLine($"CREATE{unique}INDEX `{index.Name}` ON `{index.Table}` ({columns});");
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