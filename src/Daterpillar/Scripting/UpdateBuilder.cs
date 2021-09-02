namespace Acklann.Daterpillar.Scripting
{
    public class UpdateBuilder
    {
        public UpdateBuilder(Language language) : this(null, language)
        {
        }

        public UpdateBuilder(string table, Language language = Language.SQL)
        {
            _language = language;
            _table = SqlComposer.EscapeColumn(table, language);
        }

        public UpdateBuilder Table(string tableName)
        {
            _table = SqlComposer.EscapeColumn(tableName, _language);
            return this;
        }

        public UpdateBuilder Set(string format, params object[] args)
        {
            _set = string.Format(format, args);
            return this;
        }

        public UpdateBuilder Set(string column, object value, bool overwrite = false)
        {
            if (overwrite)
            {
                _set = string.Concat(SqlComposer.EscapeColumn(column, _language), "=", SqlComposer.Serialize(value));
            }
            else
            {
                _set = string.Concat((string.IsNullOrEmpty(_set) ? null : $"{_set}, "),
                    SqlComposer.EscapeColumn(column, _language), "=", SqlComposer.Serialize(value));
            }
            return this;
        }

        public UpdateBuilder Where(string predicate)
        {
            _where = predicate;
            return this;
        }

        public UpdateBuilder Where(string column, object value, string operand = "=")
        {
            _where = string.Concat(SqlComposer.EscapeColumn(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public UpdateBuilder And(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " AND ", SqlComposer.EscapeColumn(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public UpdateBuilder Or(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " OR ", SqlComposer.EscapeColumn(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public override string ToString()
        {
            return $"UPDATE {_table} SET {_set} WHERE {_where};";
        }

        #region Operators

        public static implicit operator string(UpdateBuilder obj)
        {
            return obj.ToString();
        }

        #endregion Operators

        #region Backing Members

        private readonly Language _language;
        private string _table, _set, _where;

        #endregion Backing Members
    }
}