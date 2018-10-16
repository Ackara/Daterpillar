using Acklann.Daterpillar.Configuration;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Compilation
{
    /* documentation: https://www.sqlite.org/lang.html */

    [System.ComponentModel.Category(nameof(Syntax.SQLite))]
    public class SQLiteWriter : SqlWriter
    {
        public SQLiteWriter(Stream stream) : this(new StreamWriter(stream, System.Text.Encoding.UTF8))
        { }

        public SQLiteWriter(TextWriter writer) : base(writer, new Resolvers.SQLiteTypeResolver())
        {
            ColumnFormatString = "{0}{1}{2}{3}{4}";
            AutoIncrement = "PRIMARY KEY AUTOINCREMENT";
            CreateColumnFormatString = "ALTER TABLE {0} ADD COLUMN {1}{2}{3}{4}{5}";
            CreateIndexFormatString = "CREATE{0}INDEX IF NOT EXISTS {1} ON {2} ({3})";
            ForeignKeyFormatString = "FOREIGN KEY ({1}) REFERENCES {2}({3}) ON UPDATE {4} ON DELETE {5}";
        }

        protected override Syntax Syntax => Syntax.SQLite;

        // ==================== CREATE ==================== //

        public override void Create(Index index)
        {
            if (index.Type == IndexType.Index) base.Create(index);
            else /* IF Primary-Key */
            {
                string name = index.Table.Name;

                WriteHeaderIf($"Creating {index.Table.Name} Primary-Key");
                Writer.WriteLine("PRAGMA foreign_keys=off;");
                Writer.WriteLine("BEGIN TRANSACTION;");

                Writer.WriteLine("CREATE TABLE [_{0}_old] AS SELECT * FROM [{0}];", name);
                Writer.WriteLine("DROP TABLE [{0}];", name);

                Table clone = index.Table.Clone();
                clone.Indecies.Add(index);
                Create(clone);

                Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", name);
                Writer.WriteLine("DROP TABLE [_{0}_old];", name);

                Writer.WriteLine("COMMIT;");
                Writer.WriteLine("PRAGMA foreign_keys=on;");
                Writer.WriteLine();
                WriteEndIf();
            }
        }

        public override void Create(ForeignKey foreignKey)
        {
            /// NOTE: SQLite do not support direct alterations to a table so it will have to be recreated.

            string name = foreignKey.Table.Name;

            WriteHeaderIf($"Creating {foreignKey.Name}");
            //Writer.WriteLine("PRAGMA foreign_keys=off;");
            Writer.WriteLine("BEGIN TRANSACTION;");

            Writer.WriteLine("CREATE TABLE [_{0}_old] AS SELECT * FROM [{0}];", name);
            Writer.WriteLine("DROP TABLE [{0}];", name);

            Table clone = foreignKey.Table.Clone();
            clone.ForeignKeys.Add(foreignKey);
            Create(clone);

            Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", name);
            Writer.WriteLine("DROP TABLE [_{0}_old];", name);

            Writer.WriteLine("COMMIT;");
            //Writer.WriteLine("PRAGMA foreign_keys=on;");
            Writer.WriteLine();
            WriteEndIf();
        }

        // ==================== DROP ==================== //

        public override void Drop(Column column)
        {
            Table replacement = column.Table.Clone();
            replacement.Columns.Remove(
                replacement.Columns.Find(x => x.Name == column.Name)
                );
            string name = replacement.Name;
            string columns = string.Join(", ", replacement.Columns.Select(x => Resolver.Escape(x.Name)));

            WriteHeaderIf($"Removing {Resolver.Escape(column.Table.Name)}.{Resolver.Escape(column.Name)}");
            //Writer.WriteLine("PRAGMA foreign_keys=off;");
            Writer.WriteLine("BEGIN TRANSACTION;");

            Writer.WriteLine("CREATE TABLE [_{0}_new] AS SELECT {1} FROM [{0}];", name, columns);
            Writer.WriteLine("DROP TABLE [{0}];", name);

            Create(replacement);

            Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_new];", name);
            Writer.WriteLine("DROP TABLE [_{0}_new];", name);

            Writer.WriteLine("COMMIT;");
            //Writer.WriteLine("PRAGMA foreign_keys=on;");
            Writer.WriteLine();
            WriteEndIf();
        }

        public override void Drop(ForeignKey foreignKey)
        {
            string name = foreignKey.Table.Name;
            Table replacement = foreignKey.Table.Clone();
            replacement.ForeignKeys.Remove(replacement.ForeignKeys.Find(x => x.Name == foreignKey.Name));

            WriteHeaderIf($"Removing {Resolver.Escape(foreignKey.Name)}");
            //Writer.WriteLine("PRAGMA foreign_keys=off;");
            Writer.WriteLine("BEGIN TRANSACTION;");

            Writer.WriteLine("CREATE TABLE [_{0}_old] AS SELECT * FROM [{0}];", name);
            Writer.WriteLine("DROP TABLE [{0}];", name);

            Create(replacement);

            Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", name);
            Writer.WriteLine("DROP TABLE [_{0}_old];", name);

            Writer.WriteLine("COMMIT;");
            //Writer.WriteLine("PRAGMA foreign_keys=on;");
            Writer.WriteLine();
            WriteEndIf();
        }

        // ==================== ALTER ==================== //

        public override void Alter(Column column)
        {
            WriteHeaderIf($"Modifying {Resolver.Escape(column.Table.Name)}.{Resolver.Escape(column.Name)}");
            //Writer.WriteLine("PRAGMA foreign_keys=off;");
            Writer.WriteLine("BEGIN TRANSACTION;");

            string name = column.Table.Name;
            Writer.WriteLine("CREATE TABLE [_{0}_old] AS SELECT * FROM [{0}];", name);
            Writer.WriteLine("DROP TABLE [{0}];", name);

            Create(column.Table);

            Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", name);
            Writer.WriteLine("DROP TABLE [_{0}_old];", name);

            Writer.WriteLine("COMMIT;");
            //Writer.WriteLine("PRAGMA foreign_keys=on;");
            Writer.WriteLine();
            WriteEndIf();
        }

        public override void Rename(Column oldColumn, string newColumnName)
        {
            WriteHeaderIf($"Renaming {Resolver.Escape(oldColumn.Table.Name)}.{Resolver.Escape(oldColumn.Name)} to {newColumnName}");
            var comparer = new Equality.TableNameComparer();
            foreach (Table modifiedTable in RenameConstraints(oldColumn, newColumnName).Distinct(comparer))
            {
                rename(modifiedTable);
            }
            WriteEndIf();

            void rename(Table replacement)
            {
                //Writer.WriteLine("PRAGMA foreign_keys=off;");
                Writer.WriteLine("BEGIN TRANSACTION;");

                string tableName = replacement.Name;
                Writer.WriteLine("CREATE TABLE [_{0}_old] AS SELECT * FROM [{0}];", tableName);
                Writer.WriteLine("DROP TABLE [{0}];", tableName);

                Create(replacement);

                Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", tableName);
                Writer.WriteLine("DROP TABLE [_{0}_old];", tableName);

                Writer.WriteLine("COMMIT;");
                //Writer.WriteLine("PRAGMA foreign_keys=on;");
                Writer.WriteLine();
            }
        }
    }
}