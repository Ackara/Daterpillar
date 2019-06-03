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
            _select = string.Join(", ", columns);
            return this;
        }

        public QueryBuilder From(string table)
        {
            _from = table;
            return this;
        }

        public QueryBuilder Where(string predicate)
        {
            _where = predicate;
            return this;
        }

        public QueryBuilder GroupBy(params string[] columns)
        {
            _group = string.Join(", ", columns);
            return this;
        }

        public QueryBuilder OrderBy(params string[] columns)
        {
            _order = string.Join(", ", columns);
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

        #endregion Private Members
    }
}