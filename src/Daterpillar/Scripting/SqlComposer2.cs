using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Scripting
{
    public static class SqlComposer2
    {
        private static string ToInsertCommand(Modeling.IInsertable model, Language dialect, StringBuilder builder)
        {
            builder.Append("INSERT INTO ")
                   .Append(Linq.SqlComposer.Escape(model.GetTableName(), dialect))
                   .Append(" (")
                   .Append(string.Join(", ", (from x in model.GetColumns() select Linq.SqlComposer.Escape(x, dialect))))
                   .Append(")")
                   .Append(" VALUES ")
                   .Append("(")
                   .Append(string.Join(",", (from x in model.GetValues() select x)))
                   .Append(")")
                   ;

            return builder.ToString();
        }

        public static string ToInsertCommand(Language dialect, params Modeling.IInsertable[] modeles)
        {
            var builder = new StringBuilder();



            return builder.ToString();
        }
    }
}