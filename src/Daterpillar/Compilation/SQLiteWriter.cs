using Acklann.Daterpillar.Configuration;
using System.IO;

namespace Acklann.Daterpillar.Compilation
{
    /* documentation: https://www.sqlite.org/lang.html */

    [System.ComponentModel.Category(nameof(Configuration.Syntax.SQLite))]
    public class SQLiteWriter : SqlWriter
    {
        public SQLiteWriter(Stream stream) : this(new StreamWriter(stream, System.Text.Encoding.UTF8))
        { }

        public SQLiteWriter(TextWriter writer) : base(writer, new Resolvers.SQLiteTypeResolver())
        {
            AutoIncrement = "PRIMARY KEY AUTOINCREMENT";
        }

        protected override Syntax Syntax => Syntax.SQLite;

        public override void Create(ForeignKey foreignKey)
        {
            /// NOTE: SQLite do not support direct alterations to a table so it will have to be recreated.

            Writer.WriteLine("PRAGMA foreign_keys=off;");
            Writer.WriteLine("BEGIN TRANSACTION;");

            Writer.WriteLine("ALTER TABLE [{0}] RENAME TO [_{0}_old];", foreignKey.Table.Name);

            Table clone = foreignKey.Table.Clone();
            clone.ForeignKeys.Add(foreignKey);
            Create(clone);

            Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", foreignKey.Table.Name);
            Writer.WriteLine("DROP TABLE [_{0}_old];", foreignKey.Table.Name);

            Writer.WriteLine("COMMIT;");
            Writer.WriteLine("PRAGMA foreign_keys=on;");
            Writer.WriteLine();
        }

        public override void Drop(ForeignKey foreignKey)
        {
            /// NOTE: SQLite do not support direct alterations to a table so it will have to be recreated.

            Writer.WriteLine("PRAGMA foreign_keys=off;");
            Writer.WriteLine("BEGIN TRANSACTION;");

            Writer.WriteLine("ALTER TABLE [{0}] RENAME TO [_{0}_old];", foreignKey.Table.Name);

            Table clone = foreignKey.Table.Clone();
            clone.ForeignKeys.Remove(foreignKey);
            Create(clone);

            Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", foreignKey.Table.Name);
            Writer.WriteLine("DROP TABLE [_{0}_old];", foreignKey.Table.Name);

            Writer.WriteLine("COMMIT;");
            Writer.WriteLine("PRAGMA foreign_keys=on;");
            Writer.WriteLine();
        }

        public override void Create(Index index)
        {
            if (index.Type == IndexType.Index) base.Create(index);
            else
            {
                Writer.WriteLine("PRAGMA foreign_keys=off;");
                Writer.WriteLine("BEGIN TRANSACTION;");

                Writer.WriteLine("ALTER TABLE [{0}] RENAME TO [_{0}_old];", index.Table.Name);

                Table clone = index.Table.Clone();
                clone.Indecies.Add(index);
                Create(clone);

                Writer.WriteLine("INSERT INTO [{0}] SELECT * FROM [_{0}_old];", index.Table.Name);
                Writer.WriteLine("DROP TABLE [_{0}_old];", index.Table.Name);

                Writer.WriteLine("COMMIT;");
                Writer.WriteLine("PRAGMA foreign_keys=on;");
                Writer.WriteLine();
            }
        }
    }
}