using System.Text;

namespace Ackara.Daterpillar.Transformation.Template
{
    public class SQLiteTemplate : ITemplate
    {
        public SQLiteTemplate() : this(SqlTemplateSettings.Default)
        {
        }

        public SQLiteTemplate(SqlTemplateSettings settings)
        {
            _settings = settings;
        }

        public SQLiteTemplate(SqlTemplateSettings settings, ITypeNameResolver nameResolver)
        {
            
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
        private ITypeNameResolver _nameResolover;
        private StringBuilder _text = new StringBuilder();

        private void TransformTable(Table table)
        {
            _text.AppendLine($"CREATE TABLE IF NOT EXISTS {table.Name.ToTitle()}");
            _text.AppendLine("(");

            foreach (var column in table.Columns)
            {
                TransformColumn(column);
            }

            foreach (var foreignKey in table.ForeignKeys)
            {
                TransformForeignKey(foreignKey);
            }

            _text.TrimComma();
            _text.AppendLine(");");
        }

        private void TransformColumn(Column column)
        {
            string dataType = "";
            string modifiers = string.Join(" ", column.Modifiers);

            _text.AppendLine($"\t{column.Name.ToTitle()} {dataType} {modifiers},");
        }

        private void TransformForeignKey(ForeignKey foreignKey)
        {
            _text.AppendLine($"\tFOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate.ToText()} ON DELETE {foreignKey.OnDelete.ToText()}");
        }

        private void TransformIndex(Index index)
        {
            _text.AppendLine($"create index ;");
        }

        #endregion Private Members
    }
}