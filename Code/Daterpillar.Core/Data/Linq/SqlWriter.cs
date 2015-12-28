using System;
using System.Linq;
using System.Reflection;

namespace Gigobyte.Daterpillar.Data.Linq
{
    public static class SqlWriter
    {
        public static string ConvertToSelectCommand(EntityBase entity, QueryStyle style)
        {
            return $"SELECT * FROM {Escape(entity.TableName, style)} WHERE {GetWhereClause(entity, style)};";
        }

        public static string ConvertToDeleteCommand(EntityBase entity, QueryStyle style)
        {
            return $"DELETE FROM {Escape(entity.TableName, style)} WHERE {GetWhereClause(entity, style)};";
        }

        public static string ConvertToInsertCommand(EntityBase entity, QueryStyle style)
        {
            return $"INSERT INTO {Query.Escape(entity.TableName, style)} ({GetFields(entity, style)}) VALUES ({GetValues(entity)});";
        }

        public static string EscapeValue(object value)
        {
            if (value == null) return "''";
            else
            {
                string escapedValue;
                Type type = value.GetType();
                if (type.GetTypeInfo().IsEnum)
                {
                    escapedValue = ((int)value).ToString();
                }
                else if (type == typeof(bool))
                {
                    escapedValue = ((bool)value) ? "1" : "0";
                }
                else if (type == typeof(DateTime))
                {
                    escapedValue = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else escapedValue = value.ToString();

                escapedValue = escapedValue.Replace("'", "''");
                return $"'{escapedValue}'";
            }
        }

        internal static string Escape(string tableOrColumn, QueryStyle style)
        {
            Func<string, string, bool> notEscapedWith = (a, b) =>
            {
                return (tableOrColumn.StartsWith(a) && tableOrColumn.EndsWith(b)) == false;
            };

            switch (style)
            {
                default:
                case QueryStyle.SQL:
                    return tableOrColumn;

                case QueryStyle.TSQL:
                case QueryStyle.SQLite:
                    if (notEscapedWith("[", "]")) return String.Format("[{0}]", tableOrColumn);
                    else return tableOrColumn;

                case QueryStyle.MySQL:
                    if (notEscapedWith("`", "`")) return String.Format("`{0}`", tableOrColumn);
                    else return tableOrColumn;
            }
        }

        #region Private Members

        private static string GetFields(EntityBase entity, QueryStyle style)
        {
            var columns =
                from c in entity.GetColumns()
                where !(c.AutoIncremented || c.Value == null)
                select Query.Escape(c.Name, style);

            return String.Join(", ", columns).Trim();
        }

        private static string GetValues(EntityBase entity)
        {
            var columns =
                from c in entity.GetColumns()
                where !(c.AutoIncremented || c.Value == null)
                select EscapeValue(c.Value);

            return String.Join(", ", columns).Trim();
        }

        private static string GetWhereClause(EntityBase entity, QueryStyle style)
        {
            var keys = from k in entity.GetKeys()
                       select ($"{Escape(k.Name, style)}={EscapeValue(k.Value)}");

            return string.Join(" AND ", keys).Trim();
        }

        #endregion Private Members
    }
}