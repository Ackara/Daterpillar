using System.Linq;
using System.Text;

namespace Ackara.Daterpillar.Transformation.Template
{
    public class SQLiteTemplate : ITemplate
    {
        public SQLiteTemplate() : this(SqlTemplateSettings.Default, new SqlTypeNameResolver())
        {
        }

        public SQLiteTemplate(SqlTemplateSettings settings) : this(settings, new SqlTypeNameResolver())
        {
        }

        public SQLiteTemplate(SqlTemplateSettings settings, ITypeNameResolver typeResolver)
        {
            _settings = settings;
            _typeResolver = typeResolver;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();

            foreach (var table in schema.Tables)
            {
                TransformTable(table);
            }

            return _text.ToString();
        }

        #region Private Members

        private SqlTemplateSettings _settings;
        private ITypeNameResolver _typeResolver;
        private StringBuilder _text = new StringBuilder();

        private void TransformTable(Table table)
        {
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

            foreach (var index in table.Indexes.Where(x => x.Type == "index"))
            {
                TransformIndex(index);
            }
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
            _text.AppendLine($"\tFOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate.ToText()} ON DELETE {foreignKey.OnDelete.ToText()},");
        }

        private void TransformIndex(Index index)
        {
            string unique = index.Unique ? " UNIQUE " : " ";
            string columns = string.Join(", ", index.Columns.Select(x => $"[{x.Name}] {x.Order}"));

            _text.AppendLine($"CREATE{unique}INDEX IF NOT EXISTS {index.Name} ON [{index.Table}] ({columns});");
        }

        #endregion Private Members
    }
}