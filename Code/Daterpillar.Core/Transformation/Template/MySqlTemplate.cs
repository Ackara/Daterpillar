using System.Text;

namespace Ackara.Daterpillar.Transformation.Template
{
    public sealed class MySqlTemplate : ITemplate
    {
        public MySqlTemplate() : this(new MySqlTypeNameResolver(), true)
        {
        }

        public MySqlTemplate(ITypeNameResolver nameResolver) : this(nameResolver, true)
        {
        }

        public MySqlTemplate(ITypeNameResolver nameReslover, bool addComment)
        {
            _nameResolver = nameReslover;
            _commentsEnabled = addComment;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();

            foreach (var table in schema.Tables)
            {
                Transform(table);
            }

            return _text.ToString();
        }

        #region Private Members

        private bool _commentsEnabled;
        private ITypeNameResolver _nameResolver;
        private StringBuilder _text = new StringBuilder();

        private void Transform(Table table)
        {
            foreach (var column in table.Columns)
            {
                Transform(column);
            }

            foreach (var fkey in table.ForeignKeys)
            {
                Transform(fkey);
            }

            foreach (var index in table.Indexes)
            {
                Transform(index);
            }
        }

        private void Transform(Column column)
        {
        }

        private void Transform(ForeignKey foreignKey)
        {
        }

        private void Transform(Index index)
        {
        }

        #endregion Private Members
    }
}