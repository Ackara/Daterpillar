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
            TableCommentFormatString = "COMMENT '{0}'";
            AutoIncrement = "PRIMARY KEY AUTO_INCREMENT";
            DropIndexFormatString = "DROP INDEX {0} ON {1}";
            DropForeignKeyFormatString = "ALTER TABLE {0} DROP FOREIGN KEY {1}";
            ColumnFormatString = "{0}{1}{2}{3}{4} COMMENT '{5}'";
            CreateColumnFormatString = "ALTER TABLE {0} ADD COLUMN {1}{2}{3}{4}{5} COMMENT '{6}'";
            AlterColumnFormatString = "ALTER TABLE {0} CHANGE COLUMN {1} {1}{2}{3}{4}{5} COMMENT '{6}'";

            RenameColumnFormatString = AlterColumnFormatString;
        }


        protected override Syntax Syntax => Syntax.MySQL;


        public override void Create(Table table)
        {
            base.Create(table);
        }

        public override void Rename(Column oldColumn, string newColumnName)
        {
            Writer.Write(string.Format(AlterColumnFormatString,
                /* 0 */Resolver.Escape(oldColumn.Table.Name),
                /* 1 */Resolver.Escape(oldColumn.Name),
                /* 2 */Resolver.GetTypeName(oldColumn.DataType).WithSpace(),
                /* 3 */(oldColumn.IsNullable ? string.Empty : NotNull).WithSpace(),
                /* 4 */(oldColumn.AutoIncrement ? AutoIncrement : string.Empty).WithSpace(),
                /* 5 */(oldColumn.DefaultValue == null ? string.Empty : string.Format(DefaultFormatString, Resolver.ExpandVariables(oldColumn.DefaultValue))).WithSpace(),
                /* 6 */oldColumn.Comment.Escape()
                ).TrimEnd());
            Writer.WriteLine(';');
            Writer.WriteLine();
        }
    }
}