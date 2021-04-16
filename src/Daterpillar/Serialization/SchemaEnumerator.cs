using System.Collections;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Serialization
{
    partial class Schema
    {
        private SchemaEnumerator.Iterator _enumerator;

        /// <summary>
        /// Enumerates the <see cref="Table" /> objects in order by their dependencies.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <remarks>This function is intended to be used when generating a schema creation script. It will ensure that a table's dependency is created before it is created.</remarks>
        public IEnumerable<Table> EnumerateTables()
        {
            if (_enumerator == null) _enumerator = new SchemaEnumerator.Iterator(this);
            return _enumerator;
        }

        public class SchemaEnumerator : IEnumerator<Table>
        {
            public SchemaEnumerator(Schema schema)
            {
                _schema = schema;
                n = schema.Tables.Count;
                visited = new string[n];
            }

            public Table Current => current;
            object IEnumerator.Current => current;

            public bool MoveNext()
            {
                if (v < n && a < n)
                {
                top:
                    anchor = _schema.Tables[a];
                    current = GetDependency(anchor);

                    if (current == anchor)
                    {
                        while ((v < n && a < n) && WasVisited(current.GetIdOrName()))
                        {
                            a++;
                            current = _schema.Tables[a];
                        }
                        if (current != anchor) goto top;
                    }

                    visited[v++] = current.GetIdOrName();
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                a = v = 0;
                n = _schema.Tables.Count;
                visited = new string[n];
            }

            public void Dispose()
            {
            }

            private Table GetDependency(Table t, Table caller = null)
            {
                for (int x = 0; x < t.ForeignKeys.Count; x++)
                {
                    fk = t.ForeignKeys[x];
                    if (!WasVisited(fk.ForeignTable))
                    {
                        tmp = Find(fk.ForeignTable);
                        if (tmp == caller) return caller;// protection against circular references.
                        else if (t != tmp) return GetDependency(tmp, t);
                    }
                }

                return t; // None Found
            }

            private bool WasVisited(string name)
            {
                for (int i = 0; i < visited.Length; i++)
                    if (visited[i] == name)
                        return true;

                return false;
            }

            private Table Find(string tableName)
            {
                for (int i = 0; i < _schema.Tables.Count; i++)
                {
                    tmp = _schema.Tables[i];
                    if (tmp.GetIdOrName() == tableName) return tmp;
                }

                return null;
            }

            public class Iterator : IEnumerable<Table>
            {
                public Iterator(Schema schema)
                {
                    _enumerator = new SchemaEnumerator(schema);
                }

                private readonly SchemaEnumerator _enumerator;

                public IEnumerator<Table> GetEnumerator() => _enumerator;

                IEnumerator IEnumerable.GetEnumerator() => _enumerator;
            }

            #region Private Members

            private readonly Schema _schema;

            private string[] visited;
            private int v = 0, a = 0, n;

            private ForeignKey fk;
            private Table current, tmp, anchor;

            #endregion Private Members
        }
    }
}