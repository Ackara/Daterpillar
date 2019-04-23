using System;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Linq
{
    public static class SqlComposer
    {
        public static string[] GenerateInsertStatements(params IEntity[] entities)
        {
            if (entities == null || entities.Length == 0) return new string[0];

            IEntity entity;
            var statements = new string[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                entity = entities[i];
                statements[i] = $"INSERT INTO {entity.GetTableName()}({string.Join(",", entity.GetColumnList())})VALUES({string.Join(",", entity.GetValueList())});";
            }
            return statements;
        }

        public static string GenerateJoinedInsertStatements(params IEntity[] entities)
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
                    builder.Append($"INSERT INTO ");
                    builder.AppendLine(e.GetTableName())
                           .AppendLine($"({string.Join(", ", e.GetColumnList())})")
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
                default:
                    return name;

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