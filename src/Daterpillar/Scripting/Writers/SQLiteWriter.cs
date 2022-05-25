using Acklann.Daterpillar.Serialization;
using Acklann.Daterpillar.Scripting.Translators;
using System.Collections.Generic;
using System.IO;

namespace Acklann.Daterpillar.Scripting.Writers
{
    /* Documentation: https://www.sqlite.org/lang.html */

    [System.ComponentModel.Category(nameof(Language.SQLite))]
    public class SQLiteWriter : DDLWriter
    {
        public SQLiteWriter(Stream stream) : this(new StreamWriter(stream, System.Text.Encoding.UTF8))
        { }

        public SQLiteWriter(TextWriter writer) : base(writer, new SQLiteTranslator())
        {
            AutoIncrement = "AUTOINCREMENT";
            ColumnFormatString = "{0}{1}{2}{3}{4}";
            CreateColumnFormatString = "ALTER TABLE {0} ADD COLUMN {1}{2}{3}{4}{5}";
            CreateIndexFormatString = "CREATE{0}INDEX IF NOT EXISTS {1} ON {2} ({3})";
            ForeignKeyFormatString = "FOREIGN KEY ({1}) REFERENCES {2}({3}) ON UPDATE {4} ON DELETE {5}";
        }

        public override Language Syntax => Language.SQLite;

        // ==================== CREATE ==================== //

        public override void Create(string databaseName)
        {
        }

        public override void Create(ForeignKey foreignKey)
        {
        }

        // ==================== DROP ==================== //

        public override void Drop(Column column)
        {
        }

        public override void Drop(ForeignKey foreignKey)
        {
        }

        // ==================== ALTER ==================== //

        public override void Alter(Table oldTable, Table newTable)
        {
            WriteHeaderIf($"Modifying the [{oldTable.Name}] table.");
            Writer.WriteLine("PRAGMA foreign_keys=off;");
            Writer.WriteLine("BEGIN TRANSACTION;");

            string name = newTable.Name;
            Writer.WriteLine("CREATE TABLE [_{0}_old] AS SELECT * FROM [{0}];", name);
            Writer.WriteLine("DROP TABLE [{0}];", name);

            Create(newTable);

            Writer.WriteLine("INSERT INTO [{0}] ({2}) SELECT {1} FROM [_{0}_old];",
                name,
                string.Join(", ", oldColumns()),
                string.Join(", ", newColumns())
                );
            Writer.WriteLine("DROP TABLE [_{0}_old];", name);

            Writer.WriteLine("COMMIT;");
            Writer.WriteLine("PRAGMA foreign_keys=on;");
            Writer.WriteLine();
            WriteEndIf();

            IEnumerable<string> oldColumns()
            {
                foreach (Column newColumn in newTable.Columns)
                    foreach (Column oldColumn in oldTable.Columns)
                        if (oldColumn.IsIdentical(newColumn))
                        {
                            yield return Resolver.Escape(oldColumn.Name);
                            break;
                        }
            }

            IEnumerable<string> newColumns()
            {
                foreach (Column newColumn in newTable.Columns)
                    foreach (Column oldColumn in oldTable.Columns)
                        if (oldColumn.IsIdentical(newColumn))
                        {
                            yield return Resolver.Escape(newColumn.Name);
                            break;
                        }
            }
        }

        public override void Alter(Column column)
        {
        }

        public override void Rename(Column oldColumn, string newColumnName)
        {
        }
    }
}