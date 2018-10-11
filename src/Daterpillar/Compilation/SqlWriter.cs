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

        public bool OmitHeaders = false;

        public string IndentChars = "\t";

        protected internal string
            NotNull = "NOT NULL",
            DefaultFormatString = "DEFAULT {0}",
            AutoIncrement = "PRIMARY KEY AUTOINCREMENT",

            CreateTableFormatString = "CREATE TABLE {0}",

            ColumnFormatString = "{0} {1} {2} {3} {4} {5}",
            CreateColumnFormatString = "ALTER TABLE {0} ADD COLUMN {1} {2} {3} {4} {5} {6}",

            ForeignKeyFormatString = "FOREIGN KEY ({0}) REFERENCES {1}({2}) ON UPDATE {3} ON DELETE {4}",
            CreateForeignKeyFormatString = "ALTER TABLE {0} ADD FOREIGN KEY ({1}) REFERENCES {2}({3})  ON UPDATE {4} ON DELETE {5}",

            CreateIndexFormatString = "CREATE{0}INDEX {1} ON {2} ({3})",
            DropIndexFormatString = "DROP INDEX {0}",
            PrimaryKeyFormatString = "PRIMARY KEY ({0})",

            AlterColumnFormatString = "ALTER TABLE {0} ALTER COLUMN {1} {2} {3} {4} {5} {6}",
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
            bool notLastColumn() => i < (n - 1);

            Writer.Write(string.Format(CreateTableFormatString, Resolver.Escape(table.Name)));
            Writer.WriteLine(" (");

            //--- Columns ---//
            Column column;
            n = table.Columns.Count;
            for (i = 0; i < n; i++)
            {
                column = table.Columns[i];

                Writer.Write(IndentChars);
                Writer.Write(string.Format(ColumnFormatString,
                    Resolver.Escape(column.Name),
                    Resolver.GetTypeName(column.DataType),
                    (column.IsNullable ? string.Empty : NotNull),
                    (column.AutoIncrement ? AutoIncrement : string.Empty),
                    (column.DefaultValue == null ? string.Empty : string.Format(DefaultFormatString, column.DefaultValue)),
                    $"'{column.Comment}'"
                    ).TrimEnd());

                if /* not last-column */ (i < (n - 1)) Writer.WriteLine(",");
            }

            //--- Primary Keys ---//

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

                    if (notLastColumn()) Writer.WriteLine(",");
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
                    Resolver.Escape(fk.LocalColumn),
                    Resolver.Escape(fk.ForeignTable),
                    Resolver.Escape(fk.ForeignColumn),
                    Resolver.GetActionName(fk.OnUpdate),
                    Resolver.GetActionName(fk.OnDelete)
                    );

                if (notLastColumn()) Writer.WriteLine(",");
            }

            Writer.WriteLine();
            Writer.WriteLine($");");
            Writer.WriteLine();

            //--- Index ---//
            foreach (Index index in table.Indecies.Where(x => x.Type == IndexType.Index))
                Create(index);
        }

        public virtual void Create(Column column)
        {
            Writer.Write(string.Format(CreateColumnFormatString,
                    Resolver.Escape(column.Table.Name),
                    Resolver.Escape(column.Name),
                    Resolver.GetTypeName(column.DataType),
                    (column.IsNullable ? string.Empty : NotNull),
                    (column.AutoIncrement ? AutoIncrement : string.Empty),
                    string.Format(DefaultFormatString, column.DefaultValue),
                    column.Comment
                ).TrimEnd());
            Writer.WriteLine(";");
            Writer.WriteLine();
        }

        public virtual void Create(ForeignKey foreignKey)
        {
            Writer.Write(CreateForeignKeyFormatString,
                Resolver.Escape(foreignKey.Table.Name),
                Resolver.Escape(foreignKey.LocalColumn),
                Resolver.Escape(foreignKey.ForeignTable),
                Resolver.Escape(foreignKey.ForeignColumn),
                Resolver.GetActionName(foreignKey.OnUpdate),
                Resolver.GetActionName(foreignKey.OnDelete)
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
            throw new System.NotImplementedException();
        }

        public virtual void Drop(ForeignKey foreignKey)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Drop(Index index)
        {
            Writer.Write(DropIndexFormatString, Resolver.Escape(index.Name));
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        // ==================== ALTER ==================== //

        public virtual void Alter(Table newTable)
        {
        }

        public virtual void Alter(Column column)
        {
            Writer.Write(string.Format(AlterColumnFormatString,
                    Resolver.Escape(column.Table.Name),
                    Resolver.Escape(column.Name),
                    Resolver.GetTypeName(column.DataType),
                    (column.IsNullable ? string.Empty : NotNull),
                    (column.AutoIncrement ? AutoIncrement : string.Empty),
                    string.Format(DefaultFormatString, column.DefaultValue),
                    column.Comment
                ).TrimEnd());
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public void Rename(Table oldTable, Table newTable)
        {
            Rename(oldTable.Name, newTable.Name);
        }

        public virtual void Rename(string oldTableName, string newTableName)
        {
            Writer.Write(RenameTableFormatString, Resolver.Escape(oldTableName), Resolver.Escape(newTableName));
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public virtual void Rename(Column oldColumn, string newColumnName)
        {
            Writer.Write(RenameColumnFormatString,
                    Resolver.Escape(Resolver.Escape(oldColumn.Table.Name)),
                    Resolver.Escape(Resolver.Escape(oldColumn.Name)),
                    Resolver.Escape(newColumnName)
                );
            Writer.WriteLine(';');
            Writer.WriteLine();
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
    }
}