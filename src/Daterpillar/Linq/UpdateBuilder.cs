namespace Acklann.Daterpillar.Linq
{
    public class UpdateBuilder
    {
        public UpdateBuilder()
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

        public UpdateBuilder Limit(int value)
        {
            _limit = value;
            return this;
        }

        public override string ToString()
        {
            string statement = null;

            switch (_language)
            {
                case Language.MySQL:
                    statement = string.Concat(
                        "UPDATE ", _table,
                        " SET ", _set,
                        (string.IsNullOrEmpty(_where) ? null : $" WHERE {_where}"),
                        (_limit > 0 ? $" LIMIT {_limit}" : null),
                        ';'
                        );

                    break;

                case Language.TSQL:
                    statement = string.Concat(
                        "UPDATE ",
                        (_limit > 0 ? $" TOP ({_limit}) " : null),
                        _table,
                        " SET ", _set,
                        (string.IsNullOrEmpty(_where) ? null : $" WHERE {_where}"),
                        ';'
                        );
                    break;

                default:
                    statement = string.Concat(
                       "UPDATE ", _table,
                       " SET ", _set,
                       (string.IsNullOrEmpty(_where) ? null : $" WHERE {_where}"),
                       ';'
                       );
                    break;
            }

            return statement;
        }

        #region Backing Members

        private readonly Language _language;
        private string _table, _set, _where;
        private int _limit;

        #endregion Backing Members
    }
}