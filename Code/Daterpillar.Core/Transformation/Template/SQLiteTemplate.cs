using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class SQLiteTemplate : ITemplate
    {
        public SQLiteTemplate() : this(SQLiteTemplateSettings.Default, new SQLiteTypeNameResolver())
        {
        }

        public SQLiteTemplate(SQLiteTemplateSettings settings) : this(settings, new SQLiteTypeNameResolver())
        {
        }

        public SQLiteTemplate(SQLiteTemplateSettings settings, ITypeNameResolver typeResolver)
        {
            _typeResolver = typeResolver;
            _settings = settings;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();

            foreach (var table in schema.Tables)
            {
                Transform(table);
            }

            _text.AppendLine(schema.Script);
            return _text.ToString().Trim();
        }

        #region Private Members

        private ITypeNameResolver _typeResolver;
        private SQLiteTemplateSettings _settings;
        private StringBuilder _text = new StringBuilder();

        private void Transform(Table table)
        {
            AppendComments(table);

            if (_settings.DropTable)
            {
                _text.AppendLine($"DROP TABLE IF EXISTS [{table.Name}];");
            }

            _text.AppendLine($"CREATE TABLE IF NOT EXISTS [{table.Name}]");
            _text.AppendLine("(");

            foreach (var column in table.Columns)
            {
                Transform(column);
            }

            foreach (var index in table.Indexes.Where(x => x.IndexType == IndexType.Primary))
            {
                TransformPrimaryKey(index);
            }

            foreach (var foreignKey in table.ForeignKeys)
            {
                Transform(foreignKey);
            }

            _text.RemoveLastComma();
            _text.AppendLine(");");
            _text.AppendLine();

            foreach (var index in table.Indexes.Where(x => x.IndexType == IndexType.Index))
            {
                TransformIndex(index);
            }

            _text.AppendLine();
        }

        private void Transform(Column column)
        {
            string dataType = _typeResolver.GetName(column.DataType);
            string modifiers = string.Join(" ", column.Modifiers);
            string autoIncrement = column.AutoIncrement ? " AUTOINCREMENT" : string.Empty;

            _text.AppendLine($"\t[{column.Name}] {dataType} {modifiers}{autoIncrement},");
        }

        private void TransformPrimaryKey(Index primaryKey)
        {
            string columns = string.Join(", ", primaryKey.Columns.Select(x => $"[{x.Name}] {x.Order}"));

            _text.AppendLine($"\tPRIMARY KEY ({columns}),");
        }

        private void Transform(ForeignKey foreignKey)
        {
            _text.AppendLine($"\tFOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate} ON DELETE {foreignKey.OnDelete},");
        }

        private void TransformIndex(Index index)
        {
            string unique = index.Unique ? " UNIQUE " : " ";
            string columns = string.Join(", ", index.Columns.Select(x => $"[{x.Name}] {x.Order}"));

            _text.AppendLine($"CREATE{unique}INDEX IF NOT EXISTS {index.Name} ON [{index.Table}] ({columns});");
        }

        private void AppendComments(Table table)
        {
            if (_settings.CommentsEnabled)
            {
                if (string.IsNullOrWhiteSpace(table.Comment))
                {
                    _text.AppendLine("-- -----------------------------------");
                    _text.AppendLine($"-- {table.Name.ToUpper()} TABLE");
                    _text.AppendLine("-- -----------------------------------");
                }
                else
                {
                    _text.AppendLine("/*");
                    _text.AppendLine($"{table.Comment.Trim().AppendPeriod()}");
                    _text.AppendLine("*/");
                }
            }
        }

        #endregion Private Members
    }
}