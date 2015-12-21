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

            AppendSchemaComments(schema);
            foreach (var table in schema.Tables)
            {
                TransformTable(table);
            }

            return _text.ToString().Trim();
        }

        public void AttachScript(string path)
        {
            throw new System.NotImplementedException();
        }

        #region Private Members

        private bool _commentsEnabled;
        private ITypeNameResolver _typeResolver;
        private StringBuilder _text = new StringBuilder();

        private void TransformTable(Table table)
        {
            AppendTableComments(table);
            _text.AppendLine($"CREATE TABLE IF NOT EXISTS [{table.Name}]");
            _text.AppendLine("(");

            foreach (var column in table.Columns)
            {
                TransformColumn(column);
            }

            foreach (var index in table.Indexes.Where(x => x.Type == "primaryKey"))
            {
                TransformPrimaryKey(index);
            }

            foreach (var foreignKey in table.ForeignKeys)
            {
                TransformForeignKey(foreignKey);
            }

            _text.RemoveLastComma();
            _text.AppendLine(");");
            _text.AppendLine();

            bool hasIndex = false;

            foreach (var index in table.Indexes.Where(x => x.Type == "index"))
            {
                TransformIndex(index);
                hasIndex = true;
            }

            if (hasIndex) _text.AppendLine();
        }

        private void TransformColumn(Column column)
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

        private void TransformForeignKey(ForeignKey foreignKey)
        {
            _text.AppendLine($"\tFOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate} ON DELETE {foreignKey.OnDelete},");
        }

        private void TransformIndex(Index index)
        {
            string unique = index.Unique ? " UNIQUE " : " ";
            string columns = string.Join(", ", index.Columns.Select(x => $"[{x.Name}] {x.Order}"));

            _text.AppendLine($"CREATE{unique}INDEX IF NOT EXISTS {index.Name} ON [{index.Table}] ({columns});");
        }

        private void AppendSchemaComments(Schema schema)
        {
            if (_commentsEnabled)
            {
                int totalTables = schema.Tables.Count;
                _text.AppendLine($"/* {schema.Name} by {schema.Author} | Table(s):{totalTables} */");
                _text.AppendLine();
            }
        }

        private void AppendTableComments(Table table)
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