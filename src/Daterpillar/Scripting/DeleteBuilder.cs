namespace Acklann.Daterpillar.Scripting
{
    public class DeleteBuilder
    {
        public DeleteBuilder(Language language) : this(null, language)
        {
        }

        public DeleteBuilder(Language language, string table)
            : this(table, language) { }

        public DeleteBuilder(string table, Language language = Language.SQL)
        {
            _language = language;
            _table = SqlComposer.EscapeColumn(table, language);
        }

        public DeleteBuilder Table(string tableName)
        {
            _table = SqlComposer.EscapeColumn(tableName, _language);
            return this;
        }

        public DeleteBuilder Where(string predicate)
        {
            _where = predicate;
            return this;
        }

        public DeleteBuilder Where(string column, object value, string operand = "=")
        {
            _where = string.Concat(SqlComposer.EscapeColumn(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public DeleteBuilder And(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " AND ", SqlComposer.EscapeColumn(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public DeleteBuilder Or(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " OR ", SqlComposer.EscapeColumn(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public override string ToString()
        {
            return $"DELETE FROM {_table} WHERE {_where};";
        }

        #region Operators

        public static implicit operator string(DeleteBuilder obj)
        {
            return obj.ToString();
        }

        #endregion Operators

        #region Backing Members

        private readonly Language _language;
        private string _table, _where;

        #endregion Backing Members
    }
}