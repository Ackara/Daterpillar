using System.Linq;

namespace Gigobyte.Daterpillar.Data.Linq
{
    /// <summary>
    /// Represents a SQL query string.
    /// </summary>
    public struct Query
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> struct.
        /// </summary>
        /// <param name="style">The style.</param>
        public Query(QueryStyle style) : this()
        {
            _style = style;
        }

        /// <summary>
        /// Selects *.
        /// </summary>
        /// <returns></returns>
        public Query SelectAll()
        {
            _select = "*";
            return this;
        }

        /// <summary>
        /// SELECT [column1], [column2]....
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public Query Select(params string[] columns)
        {
            QueryStyle style = _style;
            _select = string.Join(",\n\t", columns.Select(x => Escape(x, style)));
            return this;
        }

        /// <summary>
        /// TOP n.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Query Top(int value)
        {
            _limit = value;
            return this;
        }

        /// <summary>
        /// FROM [table1], [table2]...
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <returns></returns>
        public Query From(params string[] tables)
        {
            QueryStyle style = _style;
            _from = string.Join(",\n\t", tables.Select(x => Escape(x, style)));
            return this;
        }

        /// <summary>
        /// WHERE [columA]='value'.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public Query Where(string expression)
        {
            _where = expression;
            return this;
        }

        /// <summary>
        /// GROUP BY [columnA], [columnB]...
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public Query GroupBy(params string[] columns)
        {
            QueryStyle style = _style;
            _group = string.Join(",\n\t", columns.Select(x => Escape(x, style)));
            return this;
        }

        /// <summary>
        /// ORDER BY [columnA], [columnB]...
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public Query OrderBy(params string[] columns)
        {
            QueryStyle style = _style;
            _order = string.Join(",\n\t", columns.Select(x => Escape(x, style)));
            return this;
        }

        /// <summary>
        /// LIMIT n.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Query Limit(int value)
        {
            _limit = value;
            return this;
        }

        /// <summary>
        /// Gets the SQL query.
        /// </summary>
        /// <param name="minify">if set to <c>true</c> [minify].</param>
        /// <returns></returns>
        public string GetValue(bool minify = false)
        {
            if (_select == null || _from == null) return string.Empty;
            else
            {
                string top = (_style == QueryStyle.TSQL && _limit > 0) ? (" TOP " + _limit) : string.Empty;
                string query = $"SELECT{top}\n\t{_select}\nFROM\n\t{_from}\n{GetWhere()}{GetGroupBy()}{GetOrderBy()}{GetLimit()};";
                if (minify)
                {
                    query = query.Replace("\n", " ").Replace(" \t", " ").Replace(" ;", ";");
                }

                return query;
            }
        }

        #region Private & Internal Members

        private QueryStyle _style;

        private int _limit;

        private string
            _select, _from, _where,
            _group, _order;

        internal static string Escape(string identifier, QueryStyle style)
        {
            switch (style)
            {
                case QueryStyle.MySQL:
                    identifier = "`" + identifier + "`";
                    break;

                case QueryStyle.TSQL:
                case QueryStyle.SQLite:
                    identifier = "[" + identifier + "]";
                    break;
            }

            return identifier;
        }

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
            if (_limit < 1 || _style == QueryStyle.TSQL) return string.Empty;
            else
            {
                return $"LIMIT {_limit}\n";
            }
        }

        #endregion Private & Internal Members
    }
}