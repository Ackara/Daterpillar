using System.Linq;

namespace Ackara.Daterpillar.Linq
{
    /// <summary>
    /// Represents a SQL query string.
    /// </summary>
    public struct Query
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> struct.
        /// </summary>
        /// <param name="syntax">The syntax.</param>
        public Query(Syntax syntax) : this()
        {
            _syntax = syntax;
        }

        /// <summary>
        /// Selects *.
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query SelectAll()
        {
            _select = "*";
            return this;
        }

        /// <summary>
        /// SELECT [column1], [column2]....
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query Select(params string[] columns)
        {
            Syntax syntax = _syntax;
            _select = string.Join(",\n\t", columns.Select(x => Escape(x, syntax)));
            return this;
        }

        /// <summary>
        /// TOP n.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query Top(int value)
        {
            _limit = value;
            return this;
        }

        /// <summary>
        /// FROM [table1], [table2]...
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query From(params string[] tables)
        {
            Syntax syntax = _syntax;
            _from = string.Join(",\n\t", tables.Select(x => Escape(x, syntax)));
            return this;
        }

        /// <summary>
        /// WHERE [columnA]='value'.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query Where(string expression)
        {
            _where = expression;
            return this;
        }

        /// <summary>
        /// GROUP BY [columnA], [columnB]...
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query GroupBy(params string[] columns)
        {
            Syntax syntax = _syntax;
            _group = string.Join(",\n\t", columns.Select(x => Escape(x, syntax)));
            return this;
        }

        /// <summary>
        /// ORDER BY [columnA], [columnB]...
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query OrderBy(params string[] columns)
        {
            Syntax syntax = _syntax;
            _order = string.Join(",\n\t", columns.Select(x => Escape(x, syntax)));
            return this;
        }

        /// <summary>
        /// LIMIT n.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query Limit(int value)
        {
            _limit = value;
            return this;
        }

        /// <summary>
        /// Gets the SQL query.
        /// </summary>
        /// <param name="minify">if set to <c>true</c> [minify].</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public string GetValue(bool minify = false)
        {
            if (_select == null || _from == null) return string.Empty;
            else
            {
                string top = (_syntax == Syntax.MSSQL && _limit > 0) ? (" TOP " + _limit) : string.Empty;
                string query = $"SELECT{top}\n\t{_select}\nFROM\n\t{_from}\n{GetWhere()}{GetGroupBy()}{GetOrderBy()}{GetLimit()};";
                if (minify)
                {
                    query = query.Replace("\n", " ").Replace(" \t", " ").Replace(" ;", ";");
                }

                return query;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return GetValue();
        }

        #region Private & Internal Members

        private Syntax _syntax;
        private int _limit;
        private string _select, _from, _where, _group, _order;

        internal static string Escape(string identifier, Syntax syntax)
        {
            switch (syntax)
            {
                case Syntax.MySQL:
                    identifier = "`" + identifier + "`";
                    break;

                case Syntax.MSSQL:
                case Syntax.SQLite:
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
            if (_limit < 1 || _syntax == Syntax.MSSQL) return string.Empty;
            else
            {
                return $"LIMIT {_limit}\n";
            }
        }

        #endregion Private & Internal Members
    }
}