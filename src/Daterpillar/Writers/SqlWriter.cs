using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Translators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Writers
{
    public abstract class SqlWriter : IDisposable
    {
        protected SqlWriter(TextWriter writer, ITranslator resolver)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

            Variables = new Dictionary<object, string>();
        }

        public string IndentChars = "\t";

        protected internal string
            NotNull = "NOT NULL",
            PrimaryKey = "PRIMARY KEY",
            AutoIncrement = "AUTOINCREMENT",
            DefaultFormatString = "DEFAULT {0}",

            TableCommentFormatString = string.Empty,
            CreateTableFormatString = "CREATE TABLE {0}",
            CreateDatabaseFormatString = "CREATE DATABASE {0}",

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
        protected readonly ITranslator Resolver;

        public IDictionary Variables { get; }

        public abstract Language Syntax { get; }

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

        public virtual void Create(string databaseName)
        {
            Writer.WriteLine(CreateDatabaseFormatString, Resolver.Escape(databaseName));
        }

        public virtual void Create(Schema schema)
        {
            AppendVaribales(schema);
            foreach (Table table in schema.EnumerateTables())
                Create(table);

            foreach (Script script in schema.Scripts)
                Create(script);
        }

        public virtual void Create(Table table)
        {
            int i, n;
            bool notUsingAutoKey = true;
            Writer.Write(Expand(CreateTableFormatString, Resolver.Escape(table.Name), table.Comment));
            Writer.WriteLine(" (");

            //--- Columns ---//

            Column column;
            n = table.Columns.Count;
            for (i = 0; i < n; i++)
            {
                column = table.Columns[i];
                if (column.AutoIncrement && string.Equals(column.Name, "Id", StringComparison.OrdinalIgnoreCase)) notUsingAutoKey = false;

                Writer.Write(IndentChars);
                Writer.Write(Expand(ColumnFormatString,
                    /* 0 */Resolver.Escape(column.Name),
                    /* 1 */Resolver.ConvertToString(column.DataType).WithSpace(),
                    /* 2 */(column.IsNullable ? string.Empty : NotNull).WithSpace(),
                    /* 3 */(column.AutoIncrement ? GetAutoIncrementValue(column) : string.Empty).WithSpace(),
                    /* 4 */(column.DefaultValue == null ? string.Empty : string.Format(DefaultFormatString, Resolver.ExpandVariables(column.DefaultValue))).WithSpace(),
                    /* 5 */column.Comment.Escape()
                    ));

                if /* not last-column */ (i < (n - 1)) Writer.WriteLine(",");
            }

            //--- Primary Key ---//

            if (notUsingAutoKey)
            {
                Index pk;
                n = table.Indecies.Count;
                for (i = 0; i < n; i++)
                {
                    pk = table.Indecies[i];
                    if (pk.Type == IndexType.PrimaryKey)
                    {
                        Writer.WriteLine(',');
                        Writer.Write(IndentChars);
                        Writer.Write(PrimaryKeyFormatString,
                            string.Join(", ", pk.Columns.Select(x => $"{Resolver.Escape(Expand(x.Name))} {x.Order}"))
                            );
                        break;
                    }
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
                Writer.Write(Expand(ForeignKeyFormatString,
                    Resolver.Escape(fk.Name),
                    Resolver.Escape(fk.LocalColumn),
                    Resolver.Escape(fk.ForeignTable),
                    Resolver.Escape(fk.ForeignColumn),
                    Resolver.ConvertToString(fk.OnUpdate),
                    Resolver.ConvertToString(fk.OnDelete)
                    ));
            }

            Writer.WriteLine();
            Writer.WriteLine(')');
            Writer.Write(Expand(TableCommentFormatString, table.Comment));
            Writer.WriteLine(';');
            Writer.WriteLine();

            //--- Index ---//

            foreach (Index index in table.Indecies.Where(x => x.Type == IndexType.Index))
                Create(index);
        }

        public virtual void Create(Column column)
        {
            Writer.Write(Expand(CreateColumnFormatString,
                    Resolver.Escape(column.Table.Name),
                    Resolver.Escape(column.Name),
                    Resolver.ConvertToString(column.DataType).WithSpace(),
                    (column.IsNullable ? string.Empty : NotNull).WithSpace(),
                    (column.AutoIncrement ? GetAutoIncrementValue(column) : string.Empty).WithSpace(),
                    string.Format(DefaultFormatString, GetDefaultValue(column)).WithSpace(),
                    column.Comment.Escape()
                ));
            Writer.WriteLine(";");
            Writer.WriteLine();
        }

        public virtual void Create(ForeignKey foreignKey)
        {
            Writer.Write(Expand(CreateForeignKeyFormatString,
                /* 0 */Resolver.Escape(foreignKey.Name),
                /* 1 */Resolver.Escape(foreignKey.Table.Name),
                /* 2 */Resolver.Escape(foreignKey.LocalColumn),
                /* 3 */Resolver.Escape(foreignKey.ForeignTable),
                /* 4 */Resolver.Escape(foreignKey.ForeignColumn),
                /* 5 */Resolver.ConvertToString(foreignKey.OnUpdate),
                /* 6 */Resolver.ConvertToString(foreignKey.OnDelete)
                ));
            Writer.WriteLine(";");
            Writer.WriteLine();
        }

        public virtual void Create(Index index)
        {
            var cNames = from c in index.Columns
                         select $"{Resolver.Escape(c.Name)} {c.Order}";

            Writer.Write(Expand(CreateIndexFormatString,
                (index.IsUnique ? " UNIQUE " : " "),
                Resolver.Escape(index.Name),
                Resolver.Escape(index.Table.Name),
                string.Join(", ", cNames)
                ));
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public virtual void Create(Script script)
        {
            if (script.Syntax == Syntax || script.Syntax == Language.SQL)
            {
                Writer.WriteLine(Expand(script.Content));
                Writer.WriteLine();
            }
        }

        // ==================== DROP ==================== //

        public virtual void Drop(string databaseName)
        {
            Writer.WriteLine($"DROP DATABASE IF EXISTS {Resolver.Escape(databaseName)};");
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

        public virtual void Alter(Table oldTable, Table newTable)
        {
        }

        public virtual void Alter(Column column)
        {
            Writer.Write(Expand(AlterColumnFormatString,
                    Resolver.Escape(column.Table.Name),
                    Resolver.Escape(column.Name),
                    Resolver.ConvertToString(column.DataType).WithSpace(),
                    (column.IsNullable ? string.Empty : NotNull).WithSpace(),
                    (column.AutoIncrement ? AutoIncrement : string.Empty).WithSpace(),
                    (column.DefaultValue == null ? string.Empty : string.Format(DefaultFormatString, GetDefaultValue(column))).WithSpace(),
                    column.Comment.Escape()
                ));
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public void Rename(Table oldTable, Table newTable)
        {
            Rename(oldTable.Name, newTable.Name);
        }

        public virtual void Rename(string oldTableName, string newTableName)
        {
            Writer.Write(Expand(RenameTableFormatString,
                Resolver.Escape(oldTableName),
                Resolver.Escape(newTableName)
                ));
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        public virtual void Rename(Column oldColumn, string newColumnName)
        {
            Writer.Write(Expand(RenameColumnFormatString,
                    Resolver.Escape(oldColumn.Table.Name),
                    Resolver.Escape(oldColumn.Name),
                    Resolver.Escape(newColumnName)
                ));
            Writer.WriteLine(';');
            Writer.WriteLine();
        }

        // ==================== HELPERS ==================== //

        internal void AppendVaribales(Schema schema)
        {
            foreach (Table table in schema.Tables)
            {
                AppendVariables(table);
            }

            if (string.IsNullOrEmpty(schema.Name) == false) Variables.Add("schema", schema.Name);
        }

        internal void AppendVariables(Table table)
        {
            if (string.IsNullOrEmpty(table.Id)) return;

            if (Variables.Contains(table.Id))
                throw new System.Data.DuplicateNameException(string.Format(BadKeyExceptionFormatString, table.Name, table.Id, "table"));
            else
                Variables.Add(table.Id, table.Name);

            foreach (Column column in table.Columns)
            {
                if (string.IsNullOrEmpty(column.Id)) continue;

                if (Variables.Contains(column.Id))
                    throw new System.Data.DuplicateNameException(string.Format(BadKeyExceptionFormatString, $"'{table.Name}'.'{column.Name}'", column.Id, "column"));
                else
                    Variables.Add(column.Id, column.Name);
            }
        }

        protected virtual string GetDefaultValue(Column column)
        {
            if (column.IsNullable) return Resolver.ExpandVariables(column.DefaultValue ?? Resolver.GetDefaultValue(null));
            else return Resolver.ExpandVariables(column.DefaultValue ?? Resolver.GetDefaultValue(column.DataType.Name));
        }

        protected string Expand(string format, params object[] args)
        {
            if (string.IsNullOrEmpty(format)) return string.Empty;

            format = string.Format(format, args);
            foreach (DictionaryEntry entry in Variables)
                format = Regex.Replace(format, $@"(?i)\$\({entry.Key}\)", entry.Value.ToString());

            return Environment.ExpandEnvironmentVariables(format);
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

        private const string BadKeyExceptionFormatString = "Your {0} {2} SUID ({1}) is not unique.";
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

        private string GetAutoIncrementValue(Column column)
        {
            return (string.Equals(column.Name, "id", StringComparison.OrdinalIgnoreCase) ? $"{PrimaryKey} {AutoIncrement}" : AutoIncrement);
        }

        private string GetTab(int n) => string.Concat(Enumerable.Repeat("     ", n));

        #endregion Private Members
    }
}