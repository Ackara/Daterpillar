namespace Acklann.Daterpillar.Linq
{
    public class UpdateBuilder
    {
        public UpdateBuilder(Language language) : this(null, language)
        {
        }

        public UpdateBuilder(string table, Language language = Language.SQL)
        {
            _language = language;
            _table = SqlComposer.Escape(table, language);
        }

        public UpdateBuilder Table(string tableName)
        {
            _table = SqlComposer.Escape(tableName, _language);
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
                _set = string.Concat(SqlComposer.Escape(column, _language), "=", SqlComposer.Serialize(value));
            }
            else
            {
                _set = string.Concat((string.IsNullOrEmpty(_set) ? null : $"{_set}, "),
                    SqlComposer.Escape(column, _language), "=", SqlComposer.Serialize(value));
            }
            return this;
        }

        public UpdateBuilder Where(string predicate)
        {
            _where = predicate;
            return this;
        }

        public UpdateBuilder Predicate(string column, object value, string operand = "=")
        {
            _where = string.Concat(SqlComposer.Escape(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public UpdateBuilder And(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " AND ", SqlComposer.Escape(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public UpdateBuilder Or(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " OR ", SqlComposer.Escape(column, _language), operand, SqlComposer.Serialize(value));
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