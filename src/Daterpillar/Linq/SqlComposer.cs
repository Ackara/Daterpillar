using Acklann.Daterpillar.Configuration;
using System;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Linq
{
    public static class SqlComposer
    {
        public static string GenerateInsertStatement(params ISqlObject[] entities)
        {
            ISqlObject e;
            var builder = new StringBuilder();

            if (entities == null || entities.Length == 0) return string.Empty;
            else if (entities.Length == 1)
            {
                e = entities[0];
                builder.Append($"INSERT INTO {e.TableName} ({JoinColumns(e.GetColumnList())}) VALUES ({JoinValues(e.GetValueList())});");
            }
            else
            {
                string comma = ","; int n = entities.Length;
                builder.Append($"INSERT INTO ");
                for (int i = 0; i < entities.Length; i++)
                {
                    e = entities[i];

                    if (i == 0)
                    {
                        builder.AppendLine(e.TableName);
                        builder.AppendLine($"({JoinColumns(e.GetColumnList())})");
                        builder.AppendLine("VALUES");
                    }

                    if (n == (n - 1)) comma = string.Empty;
                    builder.AppendLine($"({JoinValues(e.GetValueList())}){comma}");
                }
                builder.Append(';');
            }

            return builder.ToString();
        }

        public static string JoinColumns(params string[] columns)
        {
            return string.Join(", ", columns.Select(x => x));
        }

        public static string JoinValues(params object[] values)
        {
            return string.Join(", ", values.Select(x => x));
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
            else if (value.GetType().IsEnum)
            {
                return $"'{(int)value}'";
            }
            else
            {
                return $"'{value.ToString().Replace("'", "''")}'";
            }
        }

        public static string Escape(this string sqlName, Language syntax = Language.SQL)
        {
            switch (syntax)
            {
                default:
                    return sqlName;

                case Language.TSQL:
                case Language.SQLite:
                    return $"[{sqlName}]";

                case Language.MySQL:
                    return $"`{sqlName}`";
            }
        }
    }
}