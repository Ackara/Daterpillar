using System.Text;

namespace Acklann.Daterpillar.Linq
{
    public class QueryBuilder
    {
        public QueryBuilder(Language language = Language.SQL)
        {
            _language = language;
        }

        public QueryBuilder SelectAll()
        {
            _select = "*";
            return this;
        }

        public QueryBuilder Select(params string[] columns)
        {
            _select = Join(columns);
            return this;
        }

        public QueryBuilder From(string table)
        {
            _from = SqlComposer.Escape(table, _language);
            return this;
        }

        public QueryBuilder From(string format, params object[] args)
        {
            _from = string.Format(format, args);
            return this;
        }

        public QueryBuilder Where(string predicate)
        {
            _where = predicate;
            return this;
        }

        public QueryBuilder Predicate(string column, object value, string operand = "=")
        {
            _where = string.Concat(SqlComposer.Escape(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public QueryBuilder And(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " AND ", SqlComposer.Escape(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public QueryBuilder Or(string column, object value, string operand = "=")
        {
            _where = string.Concat(_where, " OR ", SqlComposer.Escape(column, _language), operand, SqlComposer.Serialize(value));
            return this;
        }

        public QueryBuilder GroupBy(params string[] columns)
        {
            _group = Join(columns);
            return this;
        }

        public QueryBuilder OrderBy(params string[] columns)
        {
            _order = Join(columns);
            return this;
        }

        public QueryBuilder Limit(int value)
        {
            _limit = value;
            return this;
        }

        public QueryBuilder Offset(int value)
        {
            _offset = value;
            return this;
        }

        public string ToString(Language language)
        {
            _builder.Clear().Append("SELECT");

            if (language == Language.TSQL && _limit > 0 && _offset < 1)
                _builder.AppendLine($" TOP {_limit}");
            else
                _builder.AppendLine();

            _builder.AppendLine(_select)
                    .AppendLine($"FROM {_from}");

            if (!string.IsNullOrEmpty(_where))
                _builder.AppendLine($"WHERE {_where}");

            if (!string.IsNullOrEmpty(_group))
                _builder.AppendLine($"GROUP BY {_group}");

            if (!string.IsNullOrEmpty(_order))
                _builder.AppendLine($"ORDER BY {_order}");

            if (language == Language.TSQL && _offset > 0)
                _builder.AppendLine($"OFFSET {_offset} ROWS FETCH NEXT {_limit} ROWS ONLY");

            if (language != Language.TSQL && _limit > 0)
                _builder.AppendLine($"LIMIT {_limit}");

            if (language != Language.TSQL && _offset > 0)
                _builder.AppendLine($"OFFSET {_offset}");

            _builder.Append(';');
            return _builder.ToString();
        }

        public override string ToString()
        {
            return ToString(_language);
        }

        #region Operators

        public static implicit operator string(QueryBuilder obj)
        {
            return obj.ToString(obj._language);
        }

        #endregion Operators

        #region Private Members

        private Language _language;
        private int _limit, _offset;
        private string _select, _from, _where, _group, _order;

        private StringBuilder _builder = new StringBuilder();

        private string Join(string[] columns)
        {
            var builder = new StringBuilder();
            int n = columns.Length;
            for (int i = 0; i < n; i++)
            {
                builder.Append(SqlComposer.Escape(columns[i], _language));
                if (i < (n - 1)) builder.Append(", ");
            }

            return builder.ToString();
        }

        #endregion Private Members
    }
}