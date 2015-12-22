using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gigobyte.Daterpillar.Linq
{
    /// <summary>
    /// Represents a formatted SQL query.
    /// </summary>
    public struct Query
    {
        public Query(bool minify)
            : this()
        {
            _minify = minify;
        }

        public Query Select(params string[] columns)
        {
            _select = String.Join(",\n\t", columns);
            return this;
        }

        public Query SelectAll()
        {
            _select = "*";
            return this;
        }

        public Query From(params string[] tables)
        {
            _from = String.Join(",\n\t", tables);
            return this;
        }

        public Query Join(string table, string on)
        {
            _joins = (_joins ?? "") + String.Format("\tJOIN {0} ON {1}\n", table, on);
            return this;
        }

        public Query Join(string table, string format, params object[] args)
        {
            return Join(table, String.Format(format, args));
        }

        public Query Where(string expression)
        {
            _where = expression;
            return this;
        }

        public Query Where(string format, params object[] args)
        {
            return Where(String.Format(format, args));
        }

        public Query GroupBy(params string[] columns)
        {
            _groupBy = String.Join(",\n\t", columns);
            return this;
        }

        public Query OrderBy(params string[] columns)
        {
            _orderBy = String.Join(",\n\t", columns);
            return this;
        }

        public Query Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        public Query Top(int limit)
        {
            _top = limit;
            return this;
        }

        public string GetValue()
        {
            return GetValue(_minify);
        }

        public string GetValue(bool minify)
        {
            if (String.IsNullOrEmpty(_select)) return String.Empty;
            else
            {
                string query = String.Concat(
                    Prepare("select", _select),
                    Prepare("from", _from),
                    _joins,
                    Prepare("where", _where),
                    Prepare("group by", _groupBy),
                    Prepare("order by", _orderBy));

                query = AppendLimit(query);

                return minify ? Minify(query) : query.Trim();
            }
        }

        public override string ToString()
        {
            return GetValue();
        }

        #region Private Members

        private int _limit, _top;
        private bool _minify;
        private string _select, _from, _joins, _where, _orderBy, _groupBy;

        private string Prepare(string keyword, string clause)
        {
            if (String.IsNullOrWhiteSpace(clause)) return String.Empty;
            else return clause = String.Format("{0}\n\t{1}\n", keyword.ToUpper(), clause);
        }

        private string AppendLimit(string query)
        {
            if (_limit == 0 && _top == 0) return query + ';';
            else if (_top > 0)
            {
                query = query.Insert(
                    startIndex: 6, /* The length of the SELECT keyword */
                    value: String.Format(" TOP {0}", _top));

                return query + ";";
            }
            else return String.Concat(query, "LIMIT ", _limit, "\n;");
        }

        private string Minify(string query)
        {
            /// Note: you're trimming the semicolon so you don't end up with a space before the semicolon.
            query = new Regex(@"\s+").Replace(query, " ").Trim(' ', ';');
            return query + ';';
        }

        #endregion Private Members
    }
}
