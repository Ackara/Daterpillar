using Acklann.Daterpillar.Compilation.Resolvers;
using Acklann.Daterpillar.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Compilation
{
    public abstract class SqlWriter : IDisposable
    {
        protected SqlWriter(TextWriter writer, ITypeResolver resolver)
        {
            Writer = writer;
            Resolver = resolver;
        }

        public string IndentChars = "\t";

        protected internal string
            NotNull = "NOT NULL",
            DefaultFormatString = "DEFAULT {0}",
            AutoIncrement = "PRIMARY KEY AUTOINCREMENT",

            TableCommentFormatString = string.Empty,
            CreateTableFormatString = "CREATE TABLE {0}",

            // *** Column *** //
            ColumnFormatString = "{0}{1}{2}{3}{4}",
            DropColumnFormatString = "ALTER TABLE {0} DROP COLUMN {1}",
            CreateColumnFormatString = "ALTER TABLE {0} ADD COLUMN {1}{2}{3}{4}{5}",
            AlterColumnFormatString = "ALTER TABLE {0} ALTER COLUMN {1}{2}{3}{4}{5}",

            // *** Foreign Key *** //
            DropForeignKeyFormatString = "ALTER TABLE {0} DROP CONSTRAINT {1}",
            ForeignKeyFormatString = "CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2}({3}) ON UPDATE {4} ON DELETE {5}",
            CreateForeignKeyFormatString = "ALTER TABLE {1} ADD CONSTRAINT {0} FOREIGN KEY ({2}) REFERENCES {3}({4})  ON UPDATE {5} ON DELETE {6}",

            // *** Index *** //
            DropIndexFormatString = "DROP INDEX {0}",
            PrimaryKeyFormatString = "PRIMARY KEY ({0})",
            CreateIndexFormatString = "CREATE{0}INDEX {1} ON {2} ({3})",

            // *** Rename *** //
            RenameTableFormatString = "ALTER TABLE {0} RENAME TO {1}",
            RenameColumnFormatString = "ALTER TABLE {0} RENAME COLUMN {1} TO {2}"
            ;

        protected readonly TextWriter Writer;

        protected readonly ITypeResolver Resolver;

        protected abstract Syntax Syntax { get; }

        public void Write(object value)
        {
            Writer.Write(value);
        }

        public void WriteLine(object value)
        {
            Writer.WriteLine(value);
        }

        public void Flush()
        {
            Writer.Flush();
        }

        // ==================== CREATE ==================== //

        public virtual void Create(Schema schema)
        {
            foreach (Table table in schema.Tables)
                Create(table);

            foreach (Script script in schema.Scripts)
                if (script.Syntax == Syntax.Generic || script.Syntax == Syntax)
                    Create(script);
        }

        public virtual void Create(Table table)
        {
            int i, n;
            Writer.Write(CreateTableFormatString, Resolver.Escape(table.Name), table.Comment);
            Writer.WriteLine(" (");

            //--- Columns ---//
            Column column;
            n = table.Columns.Count;
            for (i = 0; i < n; i++)
            {
                column = table.Columns[i];

                Writer.Write(IndentChars);
                Writer.Write(ColumnFormatString,
                    /* 0 */Resolver.Escape(column.Name),
                    /* 1 */Resolver.GetTypeName(column.DataType).WithSpace(),
                    /* 2 */(column.IsNullable ? string.Empty : NotNull).WithSpace(),
                    /* 3 */(column.AutoIncrement ? AutoIncrement : string.Empty).WithSpace(),
                    /* 4 */(column.DefaultValue == null ? string.Empty : string.Format(DefaultFormatString, Resolver.ExpandVariables(column.DefaultValue))).WithSpace(),
                    /* 5 */column.Comment.Escape()
                    );

                if /* not last-column */ (i < (n - 1)) Writer.WriteLine(",");
            }

            //--- Primary Key ---//

            Index pk;
            n = table.Indecies.Count;
            for (i = 0; i < n; i++)
            {
                pk = table.Indecies[i];
                if (pk.Type == IndexType.PrimaryKey)
                {
                    Writer.WriteLine(',');
                    Writer.Write(IndentChars);
                    Writer.Write(string.Format(PrimaryKeyFormatString,
                        string.Join(", ", pk.Columns.Select(x => $"{Resolver.Escape(x.Name)} {x.Order}"))
                        ));
                    break;
                }
            }

            //--- Foreign Keys ---//

            ForeignKey fk;
            n = table.ForeignKeys.Count;
            for (i = 0; i < n; i++)
            {
                fk = table.ForeignKeys[i];

                Writer.WriteLine(',');
                Writer.Write(IndentChars);
                Writer.Write(ForeignKeyFormatString,
                    Resolver.Escape(fk.Name),
                    Resolver.Escape(fk.LocalColumn),
                    Resolver.Escape(fk.ForeignTable),
                    Resolver.Escape(fk.ForeignColumn),
                    Resolver.GetActionName(fk.OnUpdate),
                    Resolver.GetActionName(fk.OnDelete)
                    );
            }

            Writer.WriteLine();
            Writer.WriteLine(')');
            Writer.Write(TableCommentFormatString, table.Comment);
            Writer.WriteLine(';');
            Writer.WriteLine();

            //--- Index ---//
            foreach (Index index in table.Indecies.Where(x => x.Type == IndexType.Index))
                Create(index);
        }

        public virtual void Create(Column column)
        {
            Writer.Write(CreateColumnFormatString,
                    Resolver.Escape(column.Table.Name),
                    Resolver.Escape(column.Name),
                    Resolver.GetTypeName(column.DataType).WithSpace(),
                    (column.IsNullable ? string.Empty : NotNull).WithSpace(),
                    (column.AutoIncrement ? AutoIncrement : string.Empty).WithSpace(),
                    string.Format(DefaultFormatString, Resolver.ExpandVariables(column.DefaultValue)).WithSpace(),
                    column.Comment.Escape()
                );
            Writer.WriteLine(";");
            Writer.WriteLine();
        }

        public virtual void Create(ForeignKey foreignKey)
        {
            Writer.Write(CreateForeignKeyFormatString,
                /* 0 */Resolver.Escape(foreignKey.Name),
                /* 1 */Resolver.Escape(foreignKey.Table.Name),
                /* 2 */Resolver.Escape(foreignKey.LocalColumn),
                /* 3 */Resolver.Escape(foreignKey.ForeignTable),
                /* 4 */Resolver.Escape(foreignKey.ForeignColumn),
                /* 5 */Resolver.GetActionName(foreignKey.OnUpdate),
                /* 6 */Resolver.GetActionName(foreignKey.OnDelete)
                );
            Writer.WriteLine(";");
            Writer.WriteLine();
        }

        public virtual void Create(Index index)
        {
            var cNames = from c in index.Columns
                         select $"{Resolver.Escape(c.Name)} {c.Order}";

            Writer.Write(CreateIndexFormatString,
                (index.IsUnique ? " UNIQUE " : " "),
                Resolver.Escape(index.Name),
                Resolver.Escape(index.Table.Name),
                string.Join(", ", cNames)
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public virtual void Create(Script script)
        {
            Writer.WriteLine(script.Content);
            Writer.WriteLine();
        }

        // ==================== DROP ==================== //

        public virtual void Drop(Schema schema)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Drop(Table table)
        {
            Writer.WriteLine("DROP TABLE {0};", Resolver.Escape(table.Name));
            Writer.WriteLine();
        }

        public virtual void Drop(Column column)
        {
            Writer.Write(DropColumnFormatString,
                Resolver.Escape(column.Table.Name),
                Resolver.Escape(column.Name)
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public virtual void Drop(ForeignKey foreignKey)
        {
            Writer.Write(DropForeignKeyFormatString,
                Resolver.Escape(foreignKey.Table.Name),
                Resolver.Escape(foreignKey.Name)
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public virtual void Drop(Index index)
        {
            Writer.Write(DropIndexFormatString,
                Resolver.Escape(index.Name),
                Resolver.Escape(index.Table.Name)
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        // ==================== ALTER ==================== //

        public virtual void Alter(Table newTable)
        {
        }

        public virtual void Alter(Column column)
        {
            Writer.Write(AlterColumnFormatString,
                    Resolver.Escape(column.Table.Name),
                    Resolver.Escape(column.Name),
                    Resolver.GetTypeName(column.DataType).WithSpace(),
                    (column.IsNullable ? string.Empty : NotNull).WithSpace(),
                    (column.AutoIncrement ? AutoIncrement : string.Empty).WithSpace(),
                    (string.Format(DefaultFormatString, Resolver.ExpandVariables(column.DefaultValue))).WithSpace(),
                    column.Comment.Escape()
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public void Rename(Table oldTable, Table newTable)
        {
            Rename(oldTable.Name, newTable.Name);
        }

        public virtual void Rename(string oldTableName, string newTableName)
        {
            Writer.Write(RenameTableFormatString,
                Resolver.Escape(oldTableName),
                Resolver.Escape(newTableName)
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public virtual void Rename(Column oldColumn, string newColumnName)
        {
            Writer.Write(RenameColumnFormatString,
                    Resolver.Escape(oldColumn.Table.Name),
                    Resolver.Escape(oldColumn.Name),
                    Resolver.Escape(newColumnName)
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Writer?.Dispose();
            }
        }

        #endregion IDisposable

        #region Private Members

        private int _indentation = 0;

        internal void WriteHeaderIf(string text, bool condition = true)
        {
            if (condition)
            {
                Writer.WriteLine($"-- {GetTab(_indentation)}{text}");
                Writer.WriteLine();
                _indentation++;
            }
        }

        internal void WriteEndIf(bool condition = true)
        {
            if (condition)
            {
                string t = GetTab(_indentation - 1);
                Writer.WriteLine($"-- {t}End{t} --");
                Writer.WriteLine();
                _indentation--;
            }
        }

        protected internal IEnumerable<Table> RemoveConstraints(Column column)
        {
            bool wasModified;
            string oldName = column.Name;
            string tableName = column.Table.Name;
            Schema schema = column.Table.Schema.Clone();

            foreach (Table table in schema.Tables)
            {
                wasModified = false;

                if (string.Equals(table.Name, tableName, StringComparison.OrdinalIgnoreCase))
                {
                    table.Columns.Remove(
                        table.Columns.Find(x => x.Name == oldName)
                        );
                    wasModified = true;
                }

                var foreignKeys = new List<ForeignKey>(table.ForeignKeys);
                foreach (ForeignKey fk in foreignKeys)
                {
                    if (fk.LocalTable == tableName && fk.LocalColumn == oldName)
                    {
                        table.ForeignKeys.Remove(fk);
                        wasModified = true;
                    }
                    else if (fk.ForeignTable == tableName && fk.ForeignColumn == oldName)
                    {
                        table.ForeignKeys.Remove(fk);
                        wasModified = true;
                    }
                }

                var indecies = new List<Index>(table.Indecies);
                foreach (Index index in indecies)
                {
                    for (int i = 0; i < index.Columns.Length; i++)
                    {
                        if (index.Columns[i].Name == oldName)
                        {
                            if (index.Columns.Length == 1)
                            {
                                table.Indecies.Remove(index);
                            }
                            else
                            {
                                index.Columns = index.Columns.Where(x => x.Name != oldName).ToArray();
                            }

                            wasModified = true;
                        }
                    }
                }

                if (wasModified) yield return table;
            }
        }

        protected internal IEnumerable<Table> RenameConstraints(Column column, string newName)
        {
            bool wasModified;
            string oldName = column.Name;
            string tableName = column.Table.Name;
            Schema schema = column.Table.Schema.Clone();

            foreach (Table table in schema.Tables)
            {
                wasModified = false;

                if (string.Equals(table.Name, tableName, StringComparison.OrdinalIgnoreCase))
                {
                    table.Columns.Find(x => x.Name == oldName).Name = newName;
                    wasModified = true;
                }

                foreach (ForeignKey fk in table.ForeignKeys)
                {
                    if (fk.LocalTable == tableName && fk.LocalColumn == oldName)
                    {
                        fk.LocalColumn = newName;
                        wasModified = true;
                    }
                    else if (fk.ForeignTable == tableName && fk.ForeignColumn == oldName)
                    {
                        fk.ForeignColumn = newName;
                        wasModified = true;
                    }
                }

                foreach (Index index in table.Indecies)
                {
                    for (int i = 0; i < index.Columns.Length; i++)
                    {
                        if (index.Columns[i].Name == oldName)
                        {
                            index.Columns[i].Name = newName;
                            wasModified = true;
                        }
                    }
                }

                if (wasModified) yield return table;
            }
        }

        private string GetTab(int n) => string.Concat(Enumerable.Repeat("     ", n));

        #endregion Private Members
    }
}