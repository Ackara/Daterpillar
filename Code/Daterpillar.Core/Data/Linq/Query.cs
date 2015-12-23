using System.Linq;

namespace Gigobyte.Daterpillar.Data.Linq
{
    /// <summary>
    /// Represents a SQL query string.
    /// </summary>
    public struct Query
    {
        public Query(SqlStyle style) : this()
        {
            _style = style;
        }

        public Query SelectAll()
        {
            _select = "*";
            return this;
        }

        public Query Select(params string[] columns)
        {
            SqlStyle style = _style;
            _select = string.Join(",\n\t", columns.Select(x => Escape(x, style)));
            return this;
        }

        public Query Top(int value)
        {
            _limit = value;
            return this;
        }

        public Query From(params string[] tables)
        {
            SqlStyle style = _style;
            _from = string.Join(",\n\t", tables.Select(x => Escape(x, style)));
            return this;
        }

        public Query Where(string expression)
        {
            _where = expression;
            return this;
        }

        public Query GroupBy(params string[] columns)
        {
            SqlStyle style = _style;
            _group = string.Join(",\n\t", columns.Select(x => Escape(x, style)));
            return this;
        }

        public Query OrderBy(params string[] columns)
        {
            SqlStyle style = _style;
            _order = string.Join(",\n\t", columns.Select(x => Escape(x, style)));
            return this;
        }

        public Query Limit(int value)
        {
            _limit = value;
            return this;
        }

        public string GetValue(bool minify = false)
        {
            if (_select == null || _from == null) return string.Empty;
            else
            {
                string top = (_style == SqlStyle.TSQL) ? (" TOP " + _limit) : string.Empty;
                string query = $"SELECT{top}\n\t{_select}\nFROM\n\t{_from}\n{GetWhere()}{GetGroupBy()}{GetOrderBy()}{GetLimit()};";
                if (minify)
                {
                    query = query.Replace("\n", " ").Replace(" \t", " ").Replace(" ;", ";");
                }

                return query;
            }
        }

        #region Private Members

        private SqlStyle _style;
        private int _limit;

        private string
            _select, _from, _where,
            _group, _order;

        private string GetWhere()
        {
            return string.IsNullOrEmpty(_where) ? string.Empty : $"WHERE\n\t{_where}\n";
        }

        private string GetGroupBy()
        {
            return string.IsNullOrEmpty(_group) ? string.Empty : $"GROUP BY\n\t{_group}\n";
        }

        private string GetOrderBy()
        {
            return string.IsNullOrEmpty(_order) ? string.Empty : $"ORDER BY\n\t{_order}\n";
        }

        private string GetLimit()
        {
            if (_limit < 1 || _style == SqlStyle.TSQL) return string.Empty;
            else
            {
                return $"LIMIT {_limit}\n";
            }
        }

        internal static string Escape(string identifier, SqlStyle style)
        {
            switch (style)
            {
                case SqlStyle.MySQL:
                    identifier = "`" + identifier + "`";
                    break;

                case SqlStyle.TSQL:
                case SqlStyle.SQLite:
                    identifier = "[" + identifier + "]";
                    break;
            }

            return identifier;
        }

        #endregion Private Members
    }
}