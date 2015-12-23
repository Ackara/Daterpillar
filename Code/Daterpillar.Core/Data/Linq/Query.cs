using System.Linq;

namespace Gigobyte.Daterpillar.Data.Linq
{
    /// <summary>
    /// Represents a formatted SQL query.
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

        public Query From(params string[] tables)
        {
            SqlStyle style = _style;
            _from = string.Join(", ", tables.Select(x => Escape(x, style)));
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
            throw new System.NotImplementedException();
        }

        internal static string Escape(string identifier, SqlStyle style)
        {
            switch (style)
            {
                case SqlStyle.MySQL:
                    identifier = "`" + identifier + "`";
                    break;

                case SqlStyle.TSQL:
                    identifier = "[" + identifier + "]";
                    break;
            }

            return identifier;
        }

        #region Private Members

        private SqlStyle _style;
        private int _limit;

        private string
            _select, _from, _where,
            _group, _order;

        #endregion Private Members
    }
}