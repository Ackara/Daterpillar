using Acklann.Daterpillar.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Compilation
{
    public class SqlMigrator
    {
        public Discrepancy[] GenerateMigrationScript(string scriptFile, Schema from, Schema to, Syntax syntax = Syntax.Generic, bool shouldOmitDropStatements = false)
        {
            using (var file = new FileStream(scriptFile, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (SqlWriter writer = _factory.CreateInstance(syntax, file))
            {
                var result = GenerateMigrationScript(writer, from, to, syntax, shouldOmitDropStatements);
                writer.Flush();
                return result;
            }
        }

        public Discrepancy[] GenerateMigrationScript(SqlWriter writer, Schema from, Schema to, Syntax syntax = Syntax.Generic, bool shouldOmitDropStatements = false)
        {
            /// Step 1:
            ///     Mark all the tables that need to be created, altered or dropped.
            /// Step 2:
            ///     Sort the tables by action then dependency.
            /// Step 3:
            ///     Write the SQL statements that will edit the schema to the stream.

            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));

            try
            {
                _discrepancies.Clear();
                Table[] left = from.Clone().Tables.ToArray();   // old
                Table[] right = to.Clone().Tables.ToArray();    // new

                // Step 1 (Analyze)
                CaptureTablesThatNeedToBeACreatedOrAltered(left, right);   // !The tables from the right
                CaptureTablesThatNeedToBeDropped(left, right);             // !The tables from the left

                // Step 2 (Sort)
                Discrepancy[] sortedTables = GetTablesSortedByDependency().ToArray();

                // Step 3 (Write)
                foreach (Discrepancy change in sortedTables)
                    WriteChanges(writer, change, shouldOmitDropStatements);

                return sortedTables;
            }
            finally
            {
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
            {
                if (action == SqlAction.None) continue;
                else
                    for (int i = 0; i < _discrepancies.Count; i++)
                    {
                        dependencyIndex = i;
                        current = _discrepancies[i];

                    retry:
                        if (current?.Action != action) continue;

                        if (hasDependency(current))
                        {
                            i--;
                            current = _discrepancies[dependencyIndex];
                            goto retry;
                        }

                        yield return current;
                        _discrepancies[dependencyIndex] = null;
                    }
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

        private void WriteChanges(SqlWriter writer, Discrepancy discrepancy, bool omitDropStatements)
        {
            if (discrepancy.WasHandled) return;

            switch (discrepancy.Action)
            {
                case SqlAction.Create:
                    writer.Create((Table)discrepancy.Value);
                    break;

                case SqlAction.Drop:
                    if (omitDropStatements == false)
                        writer.Drop((Table)discrepancy.Value);
                    break;

                case SqlAction.Alter:
                    // Dig deeper

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

        private IEnumerable<Discrepancy> FindDependencies(Table subject, SqlAction action)
        {
            Table table;
            foreach (ForeignKey fk in subject.ForeignKeys)
            {
                foreach (Discrepancy prospect in _discrepancies.Where(x => x.Action == action))
                {
                    table = (Table)prospect.Value;

                    if (string.Equals(fk.ForeignTable, table.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        //FindDependencies(table, action);
                        yield return prospect;
                    }
                }
            }
        }

        #region Private Members

        private readonly IList<Discrepancy> _discrepancies = new List<Discrepancy>();
        private readonly SqlWriterFactory _factory = new SqlWriterFactory();

        #endregion Private Members
    }
}