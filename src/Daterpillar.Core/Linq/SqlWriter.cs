using Gigobyte.Daterpillar.Data;
using System;
using System.Linq;
using System.Reflection;

namespace Gigobyte.Daterpillar.Linq
{
    public static class SqlWriter
    {
        /// <summary>
        /// Gets the SQL command to retrieve the specified <paramref name="entity"/> from a database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public static string ConvertToSelectCommand(EntityBase entity, QueryStyle style)
        {
            return $"SELECT * FROM {Escape(entity.TableName, style)} WHERE {GetWhereClause(entity, style)};";
        }

        /// <summary>
        /// Gets the SQL command to delete the specified <paramref name="entity"/> from a database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public static string ConvertToDeleteCommand(EntityBase entity, QueryStyle style)
        {
            return $"DELETE FROM {Escape(entity.TableName, style)} WHERE {GetWhereClause(entity, style)};";
        }

        /// <summary>
        /// Gets the SQL command to insert the specified <paramref name="entity"/> from a database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public static string ConvertToInsertCommand(EntityBase entity, QueryStyle style)
        {
            return $"INSERT INTO {Query.Escape(entity.TableName, style)} ({GetFields(entity, style)}) VALUES ({GetValues(entity)});";
        }

        /// <summary>
        /// Escapes the specified <paramref name="value"/> into a well formatted SQL value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
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

        #region Internal & Private Members

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

        #endregion Internal & Private Members
    }
}