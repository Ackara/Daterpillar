using Acklann.Daterpillar.Compilation.Resolvers;
using Acklann.Daterpillar.Configuration;
using System.IO;

namespace Acklann.Daterpillar.Compilation
{
    [System.ComponentModel.Category(nameof(Syntax.TSQL))]
    public class TSqlWriter : SqlWriter
    {
        public TSqlWriter(Stream stream) : this(new StreamWriter(stream), new MSSQLResolver())
        { }

        public TSqlWriter(TextWriter writer) : this(writer, new MSSQLResolver())
        { }

        public TSqlWriter(TextWriter writer, ITypeResolver resolver) : base(writer, resolver)
        {
            AutoIncrement = "PRIMARY KEY IDENTITY(1,1)";
            DropIndexFormatString = "DROP INDEX {1}.{0}";
            RenameTableFormatString = "EXEC sp_RENAME '{0}', '{1}'";
            RenameColumnFormatString = "EXEC sp_RENAME '{0}.{1}', '{2}', 'COLUMN'";
            CreateColumnFormatString = "ALTER TABLE {0} ADD {1} {2} {3} {4} {5}";
            AlterColumnFormatString = "ALTER TABLE {0} ALTER COLUMN {1} {2} {3} {4}";
        }

        protected override Syntax Syntax => Syntax.TSQL;

        public override void Rename(string oldTableName, string newTableName)
        {
            Writer.WriteLine(RenameTableFormatString,
                oldTableName,
                newTableName
                );
            Writer.WriteLine("GO");
            Writer.WriteLine();
        }

        public override void Rename(Column oldColumn, string newColumnName)
        {
            Writer.WriteLine(RenameColumnFormatString,
                    oldColumn.Table.Name,
                    oldColumn.Name,
                    newColumnName
                );
            Writer.WriteLine("GO");
            Writer.WriteLine();
        }

        public override void Alter(Column column)
        {
            Writer.WriteLine($"-- Modifying {Resolver.Escape(column.Table.Name)}.{Resolver.Escape(column.Name)}");
            Writer.WriteLine();
            base.Alter(column);
            Writer.WriteLine($"ALTER TABLE {Resolver.Escape(column.Table.Name)} ADD DEFAULT {Resolver.ExpandVariables(column.DefaultValue)} FOR {Resolver.Escape(column.Name)};");
            Writer.WriteLine();
            Writer.WriteLine("-- End --");
            Writer.WriteLine();
        }
    }
}