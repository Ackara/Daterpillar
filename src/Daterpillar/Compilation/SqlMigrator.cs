using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Equality;
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
                Discrepancy[] changes = GenerateMigrationScript(writer, from, to, syntax, shouldOmitDropStatements);
                if (changes.Length > 0) writer.Flush();
                return changes;
            }
        }

        public Discrepancy[] GenerateMigrationScript(SqlWriter writer, Schema from, Schema to, Syntax syntax = Syntax.Generic, bool shouldOmitDropStatements = false)
        {
            /// TASKS:
            /// (1) Mark all the tables that need to be created, altered or dropped.
            /// (2) Sort the tables by action then dependency.
            /// (3) Write the SQL statements that will edit the schema to the stream.

            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));

            /// 1: Analyze
            _discrepancies.Clear();
            var left = new LinkedList<Table>(from.Clone().Tables);   // old
            var right = new LinkedList<Table>(to.Clone().Tables);    // new
            CaptureTablesThatWereModified(left, right);

            /// 2: Sort
            Discrepancy[] sortedTables = GetTablesSortedByDependency().ToArray();

            /// 3: Write
            foreach (Discrepancy change in sortedTables)
                WriteChanges(writer, change, !shouldOmitDropStatements);

            return sortedTables;
        }

        // ==================== INTERNAL METHODS ==================== //

        private void CaptureTablesThatWereModified(LinkedList<Table> left, LinkedList<Table> right)
        {
            /// TASKS:
            /// (1) Drop all tables on the left that don't exist on the right.
            /// (2) Alter all tables that exist on both sides.
            /// (3) Create all the tables that exist on the right but not on the left

            bool noMatchFound;
            LinkedListNode<Table> newTable, oldTable = left.First;

            /// 1: Drop old-tables
            while (oldTable != null)
            {
                noMatchFound = true;
                newTable = right.First;

                while (newTable != null)
                {
                    if (IsMatch(oldTable.Value, newTable.Value))
                    {
                        /// 2: Alter existing tables
                        noMatchFound = false;
                        FindAllTableAlterations(oldTable.Value, newTable.Value, new Discrepancy(SqlAction.Alter, oldTable.Value, newTable.Value));
                        right.Remove(newTable);
                        break;
                    }

                    newTable = newTable.Next;
                }

                if (noMatchFound) _discrepancies.Add(new Discrepancy(SqlAction.Drop, oldTable.Value, null));
                oldTable = oldTable.Next;
            }

            /// 3: Create new-tables
            foreach (Table table in right)
                _discrepancies.Add(new Discrepancy(SqlAction.Create, null, table));
        }

        private void FindAllTableAlterations(Table left, Table right, Discrepancy discrepancy)
        {
            /// TASKS:
            /// (1) Drop all objects on the left that don't exist on the right.
            /// (2) Create all the objects that exist on the right but not on the left
            /// (3) Alter all objects that exist on both sides.

            bool noMatchFound;
            LinkedList<ISQLObject> oldItems, newItems;
            LinkedListNode<ISQLObject> oldI = null, newI = null;

            // *** COLUMNS *** //

            oldItems = new LinkedList<ISQLObject>(left.Columns);
            newItems = new LinkedList<ISQLObject>(right.Columns);
            caputure(_columnComparer, () =>
            {
                discrepancy.Add(SqlAction.Alter, oldI.Value, newI.Value);
            });

            // *** FOREIGN KEY *** //

            oldItems = new LinkedList<ISQLObject>(left.ForeignKeys);
            newItems = new LinkedList<ISQLObject>(right.ForeignKeys);
            caputure(_foreignKeyComparer, contraintHandler);

            // *** INDEX *** //

            oldItems = new LinkedList<ISQLObject>(left.Indecies);
            newItems = new LinkedList<ISQLObject>(right.Indecies);
            caputure(_indexComparer, contraintHandler);

            // *** //

            if (discrepancy.Children.Count > 0)
                _discrepancies.Add(discrepancy);

            void caputure<T>(IEqualityComparer<T> comparer, Action action)
            {
                /// 1: Drop old objects
                oldI = oldItems.First;
                while (oldI != null)
                {
                    noMatchFound = true;
                    newI = newItems.First;

                    while (newI != null)
                    {
                        if (IsMatch(oldI.Value, newI.Value))
                        {
                            noMatchFound = false;
                            /// 3: Alter objects
                            if (comparer.Equals((T)oldI.Value, (T)newI.Value) == false)
                                action?.Invoke();

                            newItems.Remove(newI);
                            break;
                        }

                        newI = newI.Next;
                    }

                    /// 1: Drop old objects
                    if (noMatchFound) discrepancy.Add(SqlAction.Drop, oldI.Value, right);
                    oldI = oldI.Next;
                }

                /// 2: Create new objects
                foreach (ISQLObject item in newItems)
                    discrepancy.Add(SqlAction.Create, left, item);
            }

            void contraintHandler()
            {
                discrepancy.Add(SqlAction.Drop, oldI.Value, right);
                discrepancy.Add(SqlAction.Create, left, newI.Value);
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

                        current.Sort();
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

        private void WriteChanges(SqlWriter writer, Discrepancy discrepancy, bool shouldIncludeDropStatements)
        {
            string separator = string.Concat(Enumerable.Repeat('-', 50));
            switch (discrepancy.Action)
            {
                case SqlAction.Create:
                    header($"Creating the {discrepancy.NewValue.GetName()} table.");
                    writer.Create((Table)discrepancy.NewValue);
                    break;
                    
                case SqlAction.Drop:
                    if (shouldIncludeDropStatements)
                    {
                        
                        writer.Drop((Table)discrepancy.OldValue);
                    }
                    break;

                case SqlAction.Alter:
                    Table oldTable = (Table)discrepancy.OldValue;
                    Table newTable = (Table)discrepancy.NewValue;
                    header($"Modifying the {oldTable.Name} table.");

                    if (string.Equals(oldTable.Name, newTable.Name, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        writer.Rename(oldTable, newTable);
                        RenameForeignKeys(oldTable, newTable.Name);
                        oldTable.Name = newTable.Name;
                    }

                    if (string.Equals(oldTable.Comment, newTable.Comment) == false)
                        writer.Alter(newTable);

                    foreach (Discrepancy item in discrepancy.Children)
                        drillDown(item);
                    break;
            }

            void drillDown(Discrepancy child)
            {
                switch (child.Value)
                {
                    // CREATE

                    case Column newColumn when child.Action == SqlAction.Create:
                        writer.Create(newColumn);
                        (child.OldValue as Table).Columns.Add(newColumn);
                        break;

                    case ForeignKey newFk when child.Action == SqlAction.Create:
                        writer.Create(newFk);
                        (child.OldValue as Table).ForeignKeys.Add(newFk);
                        break;

                    case Index newIndex when child.Action == SqlAction.Create:
                        writer.Create(newIndex);
                        (child.OldValue as Table).Indecies.Add(newIndex);
                        break;

                    // DROP

                    case Column oldColumn when child.Action == SqlAction.Drop:
                        writer.Drop(oldColumn);
                        (child.NewValue as Table).RemoveColumn(oldColumn.Name);
                        break;

                    case ForeignKey oldFk when child.Action == SqlAction.Drop:
                        writer.Drop(oldFk);
                        (child.NewValue as Table).RemoveForeignKey(oldFk.Name);
                        break;

                    case Index oldIndex when child.Action == SqlAction.Drop:
                        writer.Drop(oldIndex);
                        (child.NewValue as Table).RemoveIndex(oldIndex.Name);
                        break;

                    // ALTER

                    case Column newColumn when child.Action == SqlAction.Alter:
                        Column oldC = (Column)child.OldValue;
                        if (string.Equals(oldC.Name, newColumn.Name) == false)
                        {
                            writer.Rename(oldC, newColumn.Name);
                            oldC.Name = newColumn.Name;
                        }

                        if (_columnComparer.Equals(oldC, newColumn) == false)
                            writer.Alter(newColumn);
                        break;
                }
            }

            void header(string text)
            {
                writer.WriteLine($"-- {text}");
                writer.WriteLine(separator);
                writer.WriteLine("");
            }
        }

        private bool IsMatch(Table left, Table right)
        {
            if ((left.Id == right.Id) && (left.Id != 0 && right.Id != 0))
                return true;
            else
                return (left.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsMatch(ISQLObject left, ISQLObject right)
        {
            if (left is Column oldC)
            {
                Column newC = (Column)right;
                if (oldC.Id == newC.Id && oldC.Id != 0 && newC.Id != 0) return true;
            }

            return string.Equals(left.GetName(), right.GetName(), StringComparison.OrdinalIgnoreCase);
        }

        private void RenameForeignKeys(Table oldTable, string newName)
        {
            foreach (var item in oldTable.Schema.GetForeignKeys())
            {
                if (string.Equals(oldTable.Name, item.ForeignTable, StringComparison.OrdinalIgnoreCase))
                {
                    item.ForeignTable = newName;
                }
            }
        }

        private void WriteHeader(string text)
        {
        }

        #region Private Members

        private readonly SqlWriterFactory _factory = new SqlWriterFactory();
        private readonly IList<Discrepancy> _discrepancies = new List<Discrepancy>();
        private readonly IndexEqualityComparer _indexComparer = new IndexEqualityComparer();
        private readonly ColumnEqualityComparer _columnComparer = new ColumnEqualityComparer();
        private readonly ForeignKeyEqualityComparer _foreignKeyComparer = new ForeignKeyEqualityComparer();

        #endregion Private Members
    }
}