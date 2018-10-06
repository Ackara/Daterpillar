using Acklann.Daterpillar.Compilation.Resolvers;
using Acklann.Daterpillar.Configuration;
using System;
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
            PrimaryKeyFormatString = "PRIMARY KEY ({0})"
            ;

        protected readonly TextWriter Writer;

        protected readonly ITypeResolver Resolver;

        protected abstract Syntax Syntax { get; }

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
                    column.Comment
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

                if /* not last-column */ (i < (n - 1)) Writer.WriteLine(",");
            }

            Writer.WriteLine();
            Writer.WriteLine($");");
            Writer.WriteLine();
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        // ==================== ALTER ==================== //

        public virtual void Alter(Table oldTable, Table newTable)
        {
            throw new System.NotImplementedException();
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