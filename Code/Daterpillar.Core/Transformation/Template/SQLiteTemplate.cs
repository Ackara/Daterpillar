using System.Linq;
using System.Text;

namespace Ackara.Daterpillar.Transformation.Template
{
    public class SQLiteTemplate : ITemplate
    {
        public SQLiteTemplate() : this(new SQLiteTypeNameResolver())
        {
        }

        public SQLiteTemplate(ITypeNameResolver typeResolver, bool addComments = true)
        {
            _typeResolver = typeResolver;
            _commentsEnabled = addComments;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();

            foreach (var table in schema.Tables)
            {
                Transform(table);
            }

            return _text.ToString().Trim();
        }

        #region Private Members

        private bool _commentsEnabled;
        private ITypeNameResolver _typeResolver;
        private StringBuilder _text = new StringBuilder();

        private void Transform(Table table)
        {
            AppendComments(table);

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

            _text.AppendLine($"\t[{column.Name}] {dataType} {modifiers},");
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
            if (_commentsEnabled)
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