using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Acklann.Daterpillar.Linq
{
    /// <summary>
    /// Represents a SQL query string.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="syntax">The syntax.</param>
        public Query(Syntax syntax = Syntax.Generic)
        {
            _limit = 0;
            Syntax = syntax;
            _select = _from = _where = _group = _order = string.Empty;
        }

        /// <summary>
        /// The syntax.
        /// </summary>
        protected readonly Syntax Syntax;

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
            Syntax syntax = Syntax;
            _select = string.Join($",{newline}\t", columns.Select(x => ExtensionMethods.Escape(x, syntax)));
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
            Syntax syntax = Syntax;
            _from = string.Join($",{newline}\t", tables.Select(x => ExtensionMethods.Escape(x, syntax)));
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
            Syntax syntax = Syntax;
            _group = string.Join($",{newline}\t", columns.Select(x => ExtensionMethods.Escape(x, syntax)));
            return this;
        }

        /// <summary>
        /// ORDER BY [columnA], [columnB]...
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public Query OrderBy(params string[] columns)
        {
            Syntax syntax = Syntax;
            _order = string.Join($",{newline}\t", columns.Select(x => ExtensionMethods.Escape(x, syntax)));
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
        [Obsolete("use the ToString() method instead")]
        public string GetValue(bool minify = false)
        {
            return ToString(minify);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="minify">if set to <c>true</c> [minify].</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString(bool minify)
        {
            if (string.IsNullOrEmpty(_select) || string.IsNullOrEmpty(_from)) return string.Empty;
            else
            {
                string top = (Syntax == Syntax.MSSQL && _limit > 0) ? (" TOP " + _limit) : string.Empty;
                string query = $"SELECT{top}{newline}\t{_select}{newline}FROM{newline}\t{_from}{newline}{GetWhere()}{GetGroupBy()}{GetOrderBy()}{GetLimit()};";
                if (minify)
                {
                    query = query.Replace(newline, " ").Replace(" \t", " ").Replace(" ;", ";");
                }

                return query;
            }
        }

        #region Private & Internal Members

        private int _limit;
        private string _select, _from, _where, _group, _order, newline = Environment.NewLine;

        private string GetWhere()
        {
            return string.IsNullOrEmpty(_where) ? string.Empty : $"WHERE{newline}\t{_where}{newline}";
        }

        private string GetGroupBy()
        {
            return string.IsNullOrEmpty(_group) ? string.Empty : $"GROUP BY{newline}\t{_group}{newline}";
        }

        private string GetOrderBy()
        {
            return string.IsNullOrEmpty(_order) ? string.Empty : $"ORDER BY{newline}\t{_order}{newline}";
        }

        private string GetLimit()
        {
            if (_limit < 1 || Syntax == Syntax.MSSQL) return string.Empty;
            else
            {
                return $"LIMIT {_limit}{newline}";
            }
        }

        #endregion Private & Internal Members
    }

    public class Query<T> : Query
    {
        public Query(Syntax syntax = Syntax.Generic) : base(syntax)
        {
        }

        public new Query<T> SelectAll()
        {
            base.SelectAll();
            return this;
        }

        public Query<T> Select(params Expression<Func<T, object>>[] selectors)
        {
            Select(LinqToSqlConverter.ToColumnList(selectors).ToArray());
            return this;
        }

        public Query<T> From()
        {
            TypeInfo type = typeof(T).GetTypeInfo();
            TableAttribute attribute = type.GetCustomAttribute<TableAttribute>();
            if (attribute != null)
            {
                From((string.IsNullOrEmpty(attribute.Name) ? type.Name.Escape(Syntax) : attribute.Name.Escape(Syntax)));
            }
            else
            {
                From(type.Name);
            }

            return this;
        }

        public Query<T> From(params Type[] tables)
        {
            From(tableNames().ToArray());
            return this;

            System.Collections.Generic.IEnumerable<string> tableNames()
            {
                foreach (var type in tables)
                {
                    TableAttribute attribute = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
                    if (attribute != null)
                    {
                        yield return (string.IsNullOrEmpty(attribute.Name) ? type.Name.Escape(Syntax) : attribute.Name.Escape(Syntax));
                    }
                }
            }
        }

        public Query<T> Where(Expression<Func<T, bool>> expression)
        {
            Where(LinqToSqlConverter.ToComparisons(Syntax, expression));
            return this;
        }

        public Query<T> Where(T obj, Expression<Func<T, bool>> expression)
        {
            Where(LinqToSqlConverter.ToComparisons(Syntax, obj, expression));
            return this;
        }

        public Query<T> GroupBy(params Expression<Func<T, object>>[] selectors)
        {
            GroupBy(LinqToSqlConverter.ToColumnList(selectors).ToArray());
            return this;
        }

        public Query<T> OrderBy(params Expression<Func<T, object>>[] selectors)
        {
            OrderBy(LinqToSqlConverter.ToColumnList(selectors).ToArray());
            return this;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}