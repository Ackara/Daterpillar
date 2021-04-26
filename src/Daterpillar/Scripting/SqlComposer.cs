using System;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Scripting
{
    public static class SqlComposer
    {
        public static string ToInsertCommand(Modeling.IInsertable model, Language dialect)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO ")
                   .Append(EscapeColumn(model.GetTableName(), dialect))
                   .Append(" (")
                   .Append(string.Join(", ", (from x in model.GetColumns() select EscapeColumn(x, dialect))))
                   .Append(")")
                   .Append(" VALUES ")
                   .Append("(")
                   .Append(string.Join(", ", (from x in model.GetValues() select x)))
                   .Append(")")
                   ;

            return builder.ToString();
        }

        public static string EscapeColumn(this string columnName, Language syntax = Language.SQL)
        {
            switch (syntax)
            {
                default: return columnName;

                case Language.TSQL:
                case Language.SQLite:
                    return $"[{columnName}]";

                case Language.MySQL:
                    return $"`{columnName}`";
            }
        }

        public static string Serialize(this object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (value is bool bit)
            {
                return bit ? "'1'" : "'0'";
            }
            else if (value is TimeSpan time)
            {
                return string.Format("{0:hh}:{0:mm}:{0:ss}", time);
            }
            else if (value is DateTime date)
            {
                return $"'{date.ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            else if (value is DateTimeOffset utcDate)
            {
                return $"'{utcDate.ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            else if (value.GetType().IsEnum)
            {
                return $"'{(int)value}'";
            }
            else
            {
                return $"'{value.ToString().Replace("'", "''")}'";
            }
        }
    }
}