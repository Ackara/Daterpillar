using System;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Linq
{
    public static class SqlComposerf
    {
        public static string[] GenerateInsertStatements(Language kind, params IEntity[] entities)
        {
            if (entities == null || entities.Length == 0) return new string[0];

            IEntity entity; int cn = 0;
            var builder = new StringBuilder();
            string[] columns, statements = new string[entities.Length];
            for (int t = 0; t < entities.Length; t++)
            {
                entity = entities[t];
                builder.Append("INSERT INTO ")
                       .Append(Escape(entity.GetTableName(), kind))
                       .Append('(');

                columns = entity.GetColumnList();
                cn = columns.Length;
                for (int c = 0; c < cn; c++)
                {
                    builder.Append(Escape(columns[c], kind));
                    if (c < (cn - 1)) builder.Append(',');
                }

                builder.Append(")VALUES(")
                       .Append(string.Join(",", entity.GetValueList()))
                       .Append(");");

                statements[t] = builder.ToString();
                builder.Clear();
            }
            return statements;
        }

        public static string GenerateJoinedInsertStatements(params IEntity[] entities)
        {
            return GenerateJoinedInsertStatements(Language.SQL, entities);
        }

        public static string GenerateJoinedInsertStatements(Language kind, params IEntity[] entities)
        {
            if (entities == null || entities.Length == 0) return string.Empty;

            IEntity e;
            var builder = new StringBuilder();
            string comma = ","; int n = entities.Length;

            for (int i = 0; i < n; i++)
            {
                e = entities[i];
                if (i == 0)
                {
                    builder.Append($"INSERT INTO ")
                           .AppendLine(Escape(e.GetTableName(), kind))
                           .Append("(");

                    string[] columns = e.GetColumnList();
                    int cn = columns.Length;
                    for (int c = 0; c < cn; c++)
                    {
                        builder.Append(Escape(columns[c], kind));
                        if (c < (cn - 1)) builder.Append(", ");
                    }

                    builder.AppendLine(")")
                           .AppendLine("VALUES");
                }

                if (i == (n - 1)) comma = string.Empty;
                builder.AppendLine($"({string.Join(", ", e.GetValueList())}){comma}");
            }
            builder.Append(';');
            return builder.ToString();
        }

        public static string Escape(this string name, Language syntax = Language.SQL)
        {
            switch (syntax)
            {
                default: return name;

                case Language.TSQL:
                case Language.SQLite:
                    return $"[{name}]";

                case Language.MySQL:
                    return $"`{name}`";
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
            else if (value.GetType().IsEnum)
            {
                return $"'{(int)value}'";
            }
            else
            {
                return $"'{value.ToString().Replace("'", "''")}'";
            }
        }

        public static string JoinColumns(params string[] columns)
        {
            return string.Join(", ", columns.Select(x => Escape(x)));
        }

        public static string JoinValues(params object[] values)
        {
            return string.Join(", ", values.Select(x => Serialize(x)));
        }
    }
}