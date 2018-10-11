using Acklann.Daterpillar.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Compilation
{
    public class SqlMigrator
    {
        public Discrepancy[] GenerateMigrationScript(Stream stream, Schema from, Schema to, Syntax syntax = Syntax.Generic, bool shouldOmitDropStatements = false)
        {
            /// Step 1: 
            /// Mark all the tables that need to be created, altered or dropped.
            /// 
            /// Step 2: Sort the tables by action then dependency.
            /// 
            /// Step 3: Write the SQL statements that will edit the schema to the stream.
            
            // ownsStream: If true, the output stream is closed by the writer when done; otherwise false. The default value is true.

            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));

            IDisposable writer = null;

            try
            {
                _discrepancies.Clear();
                Table[] left = from.Clone().Tables.ToArray();// old
                Table[] right = to.Clone().Tables.ToArray();// new

                // Step 1
                CaptureTablesThatNeedToBeACreatedOrAltered(left, right);   // !The tables from the right
                CaptureTablesThatNeedToBeDropped(left, right);             // !The tables from the left

                // Step 2
                Discrepancy[] sortedTables = GetTablesSortedByDependency().ToArray();

                // Step 3
                foreach (Discrepancy change in sortedTables)
                    WriteChanges(writer, change, shouldOmitDropStatements);

                return sortedTables;
            }
            finally
            {
                stream.Dispose();
                writer.Dispose();
                _discrepancies.Clear();
            }
        }

        // ==================== INTERNAL METHODS ==================== //

        private void CaptureTablesThatNeedToBeACreatedOrAltered(Table[] left, Table[] right)
        {
            foreach (Table newTable in right)
            {
                foreach (Table oldTable in left)
                {
                    if (IsMatch(oldTable, newTable))
                    {
                        FindAllTableAlterations(oldTable, newTable, new Discrepancy(SqlAction.Alter, oldTable, newTable));
                        break;
                    }
                }
                _discrepancies.Add(new Discrepancy(SqlAction.Create, null, newTable));
            }
        }

        private void FindAllTableAlterations(Table left, Table right, Discrepancy discrepancy)
        {
            _discrepancies.Add(discrepancy);
        }

        private void CaptureTablesThatNeedToBeDropped(Table[] left, Table[] right)
        {
            /// Dropping the old tables; the ones on the left.

            foreach (Table oldTable in left)
            {
                foreach (Table newTable in right)
                {
                    if (oldTable.Name.Equals(newTable.Name, StringComparison.OrdinalIgnoreCase)) break;
                }
                _discrepancies.Add(new Discrepancy(SqlAction.Drop, oldTable, null));
            }
        }

        private IEnumerable<Discrepancy> GetTablesSortedByDependency()
        {
            Discrepancy current;
            int dependencyIndex;

            foreach (SqlAction action in Enum.GetValues(typeof(SqlAction)))
                if (action == SqlAction.None)
                    continue;
                else
                    for (int index = 0; index < _discrepancies.Count; index++)
                    {
                        dependencyIndex = index;
                        current = _discrepancies[index];

                    retry:
                        if (current?.Action != action) continue;

                        if (hasDependency(current))
                        {
                            current = _discrepancies[dependencyIndex];
                            goto retry;
                        }

                        yield return current;
                        _discrepancies[dependencyIndex] = null;
                    }

            bool hasDependency(Discrepancy item)
            {
                IEnumerable<string> foreignKeys = from f in ((item.Value as Table).ForeignKeys)
                                                  select f.ForeignTable;

                foreach (string fk in foreignKeys)
                    for (int i = 0; i < _discrepancies.Count; i++)
                        if (_discrepancies[i] != null && _discrepancies[i].Value.GetName().Equals(fk, StringComparison.OrdinalIgnoreCase))
                        {
                            dependencyIndex = i;
                            return true;
                        }

                return false;
            }
        }

        private void WriteChanges(object writer, Discrepancy discrepancy, bool omitDropStatements)
        {
            switch (discrepancy.Action)
            {
                case SqlAction.Create:
                    // write.Create(table)
                    break;

                case SqlAction.Alter:
                    // Dig deeper
                    break;

                case SqlAction.Drop:
                    if (omitDropStatements == false)
                    {
                        // write.Drop(table);
                    }
                    break;
            }
        }

        private bool IsMatch(Table left, Table right)
        {
            if (left.Id == right.Id)
                return true;
            else
                return (left.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsMatch(Column left, Column right)
        {
            if (left.Id == right.Id)
                return true;
            else
                return (left.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase));
        }

        #region Private Members

        private readonly IList<Discrepancy> _discrepancies = new List<Discrepancy>();
        private readonly object _factory;
        private object _writer;

        #endregion Private Members
    }
}