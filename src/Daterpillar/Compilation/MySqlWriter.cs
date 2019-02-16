using Acklann.Daterpillar.Compilation.Resolvers;
using Acklann.Daterpillar.Configuration;
using System.IO;

namespace Acklann.Daterpillar.Compilation
{
    [System.ComponentModel.Category(nameof(Syntax.MySQL))]
    public class MySqlWriter : SqlWriter
    {
        public MySqlWriter(Stream stream) : this(new StreamWriter(stream), new MySQLTypeResolver())
        { }

        public MySqlWriter(TextWriter writer) : this(writer, new MySQLTypeResolver())
        { }

        public MySqlWriter(TextWriter writer, ITypeResolver resolver) : base(writer, resolver)
        {
            AutoIncrement = "AUTO_INCREMENT";
            TableCommentFormatString = "COMMENT '{0}'";
            DropIndexFormatString = "DROP INDEX {0} ON {1}";
            ColumnFormatString = "{0}{1}{2}{3}{4} COMMENT '{5}'";
            DropForeignKeyFormatString = "ALTER TABLE {0} DROP FOREIGN KEY {1}";
            CreateColumnFormatString = "ALTER TABLE {0} ADD COLUMN {1}{2}{3}{4}{5} COMMENT '{6}'";
            AlterColumnFormatString = "ALTER TABLE {0} CHANGE COLUMN {1} {1}{2}{3}{4}{5} COMMENT '{6}'";
            RenameColumnFormatString = "ALTER TABLE {0} CHANGE COLUMN {1} {2}{3}{4}{5}{6} COMMENT '{7}'";
        }

        public override Syntax Syntax => Syntax.MySQL;

        public override void Rename(ColumnDeclaration oldColumn, string newColumnName)
        {
            Writer.Write(Expand(RenameColumnFormatString,
                /* 0 */Resolver.Escape(oldColumn.Table.Name),
                /* 1 */Resolver.Escape(oldColumn.Name),
                /* 2 */Resolver.Escape(newColumnName),
                /* 3 */Resolver.GetTypeName(oldColumn.DataType).WithSpace(),
                /* 4 */(oldColumn.IsNullable ? string.Empty : NotNull).WithSpace(),
                /* 5 */(oldColumn.AutoIncrement ? AutoIncrement : string.Empty).WithSpace(),
                /* 6 */(oldColumn.DefaultValue == null ? string.Empty : string.Format(DefaultFormatString, Resolver.ExpandVariables(oldColumn.DefaultValue))).WithSpace(),
                /* 7 */oldColumn.Comment.Escape()
                ));
            Writer.WriteLine(';');
            Writer.WriteLine();
        }
    }
}