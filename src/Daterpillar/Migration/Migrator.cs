using Acklann.Daterpillar.Writers;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Equality;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Migration
{
    public class Migrator
    {
        public Discrepancy[] GenerateMigrationScript(string scriptFile, Schema from, Schema to, Syntax syntax = Syntax.Generic, bool shouldOmitDropStatements = false)
        {
            Utility.CreateDirectory(scriptFile);
            using (var file = new FileStream(scriptFile, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (SqlWriter writer = _factory.CreateInstance(syntax, file))
            {
                Discrepancy[] changes = GenerateMigrationScript(writer, from, to, shouldOmitDropStatements);
                if (changes.Length > 0) writer.Flush();
                return changes;
            }
        }

        public Discrepancy[] GenerateMigrationScript(SqlWriter writer, Schema from, Schema to, bool shouldOmitDropStatements = false)
        {
            /// TASKS:
            /// (1) Mark all the tables that need to be created, altered or dropped.
            /// (2) Sort the tables by action then dependency.
            /// (3) Write the SQL statements that will edit the schema to the stream.
            /// (3.1) Ensure the scripts are inserted at the right location.

            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));

            /// 1: Analyze
            _discrepancies.Clear();
            Schema left = from.Clone();   // old
            Schema right = to.Clone();    // new
            var scripts = new LinkedList<Script>(CaputureNewScripts(left.Scripts, right.Scripts));
            CaptureTablesThatWereModified(new LinkedList<Table>(left.Tables), new LinkedList<Table>(right.Tables));

            /// 2: Sort
            Discrepancy[] sortedTables = GetTablesSortedByDependency().ToArray();

            /// 3: Write
            AppendVaribales(writer, right);

            foreach (Discrepancy change in sortedTables)
                WriteChanges(writer, change, scripts, !shouldOmitDropStatements);

            foreach (Script script in scripts)
                writer.Create(script);

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
                    if (oldTable.Value.IsIdentical(newTable.Value))
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
            LinkedList<ISqlStatement> oldItems, newItems;
            LinkedListNode<ISqlStatement> oldI = null, newI = null;

            // *** COLUMNS *** //

            oldItems = new LinkedList<ISqlStatement>(left.Columns);
            newItems = new LinkedList<ISqlStatement>(right.Columns);
            caputure(_columnComparer, () =>
            {
                discrepancy.Add(SqlAction.Alter, oldI.Value, newI.Value);
            });

            // *** FOREIGN KEY *** //

            oldItems = new LinkedList<ISqlStatement>(left.ForeignKeys);
            newItems = new LinkedList<ISqlStatement>(right.ForeignKeys);
            caputure(_foreignKeyComparer, contraintHandler);

            // *** INDEX *** //

            oldItems = new LinkedList<ISqlStatement>(left.Indecies);
            newItems = new LinkedList<ISqlStatement>(right.Indecies);
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
                        if (oldI.Value.IsIdentical(newI.Value))
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
                    if (noMatchFound) discrepancy.Add(SqlAction.Drop, oldI.Value, left);
                    oldI = oldI.Next;
                }

                /// 2: Create new objects
                foreach (ISqlStatement item in newItems)
                    discrepancy.Add(SqlAction.Create, left, item);
            }

            void contraintHandler()
            {
                discrepancy.Add(SqlAction.Drop, oldI.Value, right);
                discrepancy.Add(SqlAction.Create, left, newI.Value);
            }
        }

        private IEnumerable<Script> CaputureNewScripts(IEnumerable<Script> left, IEnumerable<Script> right)
        {
            bool noMatchFound;
            foreach (Script newScript in right)
            {
                noMatchFound = true;
                foreach (Script oldScript in left)
                {
                    if (oldScript.Syntax == newScript.Syntax && string.Equals(newScript.Name, oldScript.Name, StringComparison.OrdinalIgnoreCase))
                        noMatchFound = false;
                }

                if (noMatchFound) yield return newScript;
            }
        }

        private IEnumerable<Discrepancy> GetTablesSortedByDependency()
        {
            Discrepancy current;
            int dependencyIndex, previousDependencyIndex = -1;

            for (int i = 0; i < _discrepancies.Count; i++)
            {
                dependencyIndex = i;
                previousDependencyIndex = -1;
                current = _discrepancies[i];

            retry:
                if (current == null) continue;

                if (hasDependency(current))
                {
                    current = _discrepancies[dependencyIndex];
                    goto retry;
                }

                current.Sort();
                yield return current;
                _discrepancies[dependencyIndex] = null;
                if (dependencyIndex != i) i--; // Repositions the index to the parent, due to it having dependency.
            }

            bool hasDependency(Discrepancy item)
            {
                if (_discrepancies.Count <= 1) return false;

                IEnumerable<string> foreignKeys = from f in ((item.Value as Table).ForeignKeys)
                                                  select f.ForeignTable;

                foreach (string fk in foreignKeys)
                    for (int i = 0; i < _discrepancies.Count; i++)
                        if (_discrepancies[i] != null && _discrepancies[i].Value.GetName().Equals(fk, StringComparison.OrdinalIgnoreCase))
                        {
                            if (i == previousDependencyIndex) return false; // Guards against circular references

                            previousDependencyIndex = dependencyIndex;
                            dependencyIndex = i;
                            return true;
                        }

                return false;
            }
        }

        private void WriteChanges(SqlWriter writer, Discrepancy discrepancy, LinkedList<Script> scripts, bool shouldIncludeDropStatements)
        {
            Script[] associatedScripts = FindAssociatedScripts(scripts, discrepancy, writer.Syntax).ToArray();
            int children = associatedScripts.Length;

            // BEFORE
            foreach (Script script in associatedScripts)
                if (!string.IsNullOrEmpty(script?.Before))
                    writer.Create(script);

            switch (discrepancy.Action)
            {
                case SqlAction.Create:
                    children += ((Table)discrepancy.NewValue).Indecies.Count(x => x.Type == IndexType.Index);
                    writer.WriteHeaderIf($"Creating the {discrepancy.NewValue.GetName()} table", (children > 0));
                    writer.Create((Table)discrepancy.NewValue);
                    break;

                case SqlAction.Drop:
                    if (shouldIncludeDropStatements)
                        writer.Drop((Table)discrepancy.OldValue);
                    break;

                case SqlAction.Alter:
                    Table oldTable = (Table)discrepancy.OldValue;
                    Table newTable = (Table)discrepancy.NewValue;
                    int nChanges = discrepancy.Children.Count;

                    if (string.Equals(oldTable.Name, newTable.Name, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        writer.Rename(oldTable, newTable);
                        RenameForeignKeysReferencedTable(oldTable.Schema, newTable.Name);
                        oldTable.Name = newTable.Name;
                    }

                    /// NOTE: Because SQLite do not support native functions for modifying constraints.
                    /// I have to reconstruct the entire table.
                    if ((writer.Syntax == Syntax.SQLite && discrepancy.Children.IsNotEmpty()) || string.Equals(oldTable.Comment, newTable.Comment) == false)
                    {
                        writer.Alter(oldTable, newTable);
                        break;
                    }

                    writer.WriteHeaderIf($"Modifying the {oldTable.Name} table", (nChanges > 1));
                    foreach (Discrepancy child in discrepancy.Children)
                        drillDown(child);
                    writer.WriteEndIf(nChanges > 1);
                    break;
            }

            // AFTER
            foreach (Script script in associatedScripts)
                if (!string.IsNullOrEmpty(script.After))
                    writer.Create(script);

            writer.WriteEndIf(children > 0);

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
                        if (string.Equals(oldC.Name, newColumn.Name, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            writer.Rename(oldC, newColumn.Name);
                            oldC.Name = newColumn.Name;
                        }

                        if (_columnComparer.Equals(oldC, newColumn) == false)
                        {
                            writer.Alter(newColumn);
                            oldC.Overwrite(newColumn);
                        }
                        break;
                }
            }
        }

        private void AppendVaribales(SqlWriter writer, Schema schema)
        {
            string errorMsg = "Your {0} {2} SUID ({1}) is not unique.";
            foreach (Table table in schema.Tables)
            {
                if (string.IsNullOrEmpty(table.Id)) continue;

                if (writer.Variables.Contains(table.Id))
                    throw new System.Data.DuplicateNameException(string.Format(errorMsg, table.Name, table.Id, "table"));
                else
                    writer.Variables.Add(table.Id, table.Name);

                foreach (Column column in table.Columns)
                {
                    if (string.IsNullOrEmpty(column.Id)) continue;

                    if (writer.Variables.Contains(column.Id))
                        throw new System.Data.DuplicateNameException(string.Format(errorMsg, $"'{table.Name}'.'{column.Name}'", column.Id, "column"));
                    else
                        writer.Variables.Add(column.Id, column.Name);
                }
            }

            if (string.IsNullOrEmpty(schema.Name) == false) writer.Variables.Add("schema", schema.Name);
        }

        private IEnumerable<Script> FindAssociatedScripts(LinkedList<Script> propspects, Discrepancy discrepancy, Syntax syntax)
        {
            string suid = (discrepancy.Value as Table).Id;
            if (string.IsNullOrEmpty(suid)) yield break;

            Script script;
            bool matchFound;
            LinkedListNode<Script> current = propspects.First, prev;

            while (current != null)
            {
                script = current.Value;

                matchFound =
                    (script.Before == suid || script.After == suid)
                    &&
                    (script.Syntax == Syntax.Generic || script.Syntax == syntax);

                prev = current;
                current = current.Next;
                if (matchFound)
                {
                    propspects.Remove(prev);
                    yield return script;
                }
            }
        }

        private void RenameForeignKeysReferencedTable(Schema oldSchema, string newName)
        {
            foreach (ForeignKey fk in oldSchema.GetForeignKeys())
            {
                if (string.Equals(oldSchema.Name, fk.ForeignTable, StringComparison.OrdinalIgnoreCase))
                {
                    fk.ForeignTable = newName;
                }
            }
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