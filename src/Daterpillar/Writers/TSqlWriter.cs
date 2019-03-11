using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Translators;
using System.IO;

namespace Acklann.Daterpillar.Writers
{
    [System.ComponentModel.Category(nameof(Language.TSQL))]
    public class TSqlWriter : SqlWriter
    {
        public TSqlWriter(Stream stream) : this(new StreamWriter(stream), new TSQLTranslator())
        { }

        public TSqlWriter(TextWriter writer) : this(writer, new TSQLTranslator())
        { }

        public TSqlWriter(TextWriter writer, ITranslator resolver) : base(writer, resolver)
        {
            AutoIncrement = "IDENTITY(1,1)";
            DropIndexFormatString = "DROP INDEX {1}.{0}";
            RenameTableFormatString = "EXEC sp_RENAME '{0}', '{1}'";
            CreateColumnFormatString = "ALTER TABLE {0} ADD {1}{2}{3}{4}{5}";
            AlterColumnFormatString = "ALTER TABLE {0} ALTER COLUMN {1}{2}{3}";
            RenameColumnFormatString = "EXEC sp_RENAME '{0}.{1}', '{2}', 'COLUMN'";
        }

        public override Language Syntax => Language.TSQL;

        public override void Rename(string oldTableName, string newTableName)
        {
            Writer.WriteLine(Expand(RenameTableFormatString,
                oldTableName,
                newTableName
                ));
            Writer.WriteLine("GO");
            Writer.WriteLine();
        }

        public override void Rename(Column oldColumn, string newColumnName)
        {
            Writer.WriteLine(Expand(RenameColumnFormatString,
                    oldColumn.Table.Name,
                    oldColumn.Name,
                    newColumnName
                ));
            Writer.WriteLine("GO");
            Writer.WriteLine();
        }

        public override void Alter(Column column)
        {
            WriteHeaderIf(Expand($"Modifying {Resolver.Escape(column.Table.Name)}.{Resolver.Escape(column.Name)}"), column.DefaultValue != null);
            base.Alter(column);
            if (column.DefaultValue != null)
            {
                Writer.WriteLine(Expand($"ALTER TABLE {Resolver.Escape(column.Table.Name)} ADD DEFAULT {Resolver.ExpandVariables(column.DefaultValue)} FOR {Resolver.Escape(column.Name)};"));
                Writer.WriteLine();
            }
            WriteEndIf(column.DefaultValue != null);
        }
    }
}