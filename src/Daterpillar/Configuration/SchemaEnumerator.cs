using System.Collections;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Configuration
{
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
                    while ((v < n && a < n) && WasVisited(current.Name))
                    {
                        a++;
                        current = _schema.Tables[a];
                    }
                    if (current != anchor) goto top;
                }

                visited[v++] = current.Name;
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

        #region Private Members

        private readonly Schema _schema;

        private string[] visited;
        private int v = 0, a = 0, n;

        private ForeignKey fk;
        private Table current, tmp, anchor;

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
                if (tmp.Name == tableName) return tmp;
            }

            return null;
        }

        #endregion Private Members
    }
}