using Acklann.Daterpillar.Configuration;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar
{
    public static class Helper
    {
        public static string Escape(this string text) => text?.Replace("'", @"\'");

        /// <summary>
        /// Determines whether the specified list is empty.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if the specified list is empty; otherwise, <c>false</c>.</returns>
        public static bool IsEmpty(this ICollection list)
        {
            return list.Count == 0;
        }

        /// <summary>
        /// Determines whether the specified list is not empty.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if the specified list is not empty; otherwise, <c>false</c>.</returns>
        public static bool IsNotEmpty(this ICollection list)
        {
            return list?.Count > 0;
        }

        /// <summary>
        /// Converts the specified string to pascal case.
        /// </summary>
        /// <param name="text">The string to convert to pascal case.</param>
        /// <returns>The specified string converted to pascal case.</returns>
        public static string ToPascal(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            else if (text.Length == 1) return text.ToUpperInvariant();
            else
            {
                var pascal = new StringBuilder();
                ReadOnlySpan<char> span = text.AsSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == ' ' || span[i] == '_')
                        continue;
                    else if (i == 0)
                        pascal.Append(char.ToUpperInvariant(span[i]));
                    else if (span[i - 1] == ' ' || span[i - 1] == '_')
                        pascal.Append(char.ToUpperInvariant(span[i]));
                    else
                        pascal.Append(span[i]);
                }

                return pascal.ToString();
            }
        }

        /// <summary>
        /// Converts the specified string to camel case.
        /// </summary>
        /// <param name="text">The string to convert to camel case.</param>
        /// <returns>The specified string converted to camel case.</returns>
        public static string ToCamel(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            else if (text.Length == 1) return text.ToUpperInvariant();
            else
            {
                var camel = new StringBuilder();
                ReadOnlySpan<char> span = text.AsSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == ' ' || span[i] == '_')
                        continue;
                    else if (i == 0)
                        camel.Append(char.ToLowerInvariant(span[i]));
                    else if (span[i - 1] == ' ' || span[i - 1] == '_')
                        camel.Append(char.ToUpperInvariant(span[i]));
                    else
                        camel.Append(span[i]);
                }

                return camel.ToString();
            }
        }

        /// <summary>
        /// Converts the specified string to camel case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The specified string converted to snake case.</returns>
        public static string ToSnake(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            else if (text.Length == 1) return text.ToUpperInvariant();
            else
            {
                var span = text.AsSpan();
                var snake = new StringBuilder();

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == ' ')
                        snake.Append('_');
                    else if (char.IsUpper(span[i]) && i > 0 && (span[i - 1] != ' ' && span[i - 1] != '_'))
                    {
                        snake.Append('_');
                        snake.Append(char.ToLowerInvariant(span[i]));
                    }
                    else
                        snake.Append(char.ToLowerInvariant(span[i]));
                }

                return snake.ToString();
            }
        }

        // ==================== INTERNAL MEMBERS ==================== //

        internal static string WithSpace(this string text) => string.IsNullOrEmpty(text) ? string.Empty : $" {text.Trim()}";

        internal static void CreateDirectory(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            string folder = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }

        internal static bool IsIdentical(this Table x, Table y)
        {
            return
                (x.Id == y?.Id && !string.IsNullOrEmpty(x?.Id) && !string.IsNullOrEmpty(y?.Id))
                ||
                string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase)
                ;
        }

        internal static bool IsIdentical(this Column x, Column y)
        {
            return
                (x.Id == y?.Id && !string.IsNullOrEmpty(x?.Id) && !string.IsNullOrEmpty(y?.Id))
                ||
                string.Equals(x.Name, y?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        internal static bool IsIdentical(this Script x, Script y)
        {
            return
                string.Equals(x.Name, y?.Name, StringComparison.OrdinalIgnoreCase)
                &&
                x.Syntax == y.Syntax;
        }

        internal static bool IsIdentical(this ISqlObject x, ISqlObject y)
        {
            if (x is Table)
                return IsIdentical((Table)x, (Table)y);
            else if (x is Column)
                return IsIdentical((Column)x, (Column)y);
            else
                return string.Equals(x.GetName(), y.GetName(), StringComparison.OrdinalIgnoreCase);
        }

        internal static string GetId(this MemberInfo member)
        {
            return (!(member.GetCustomAttribute(typeof(StaticIdAttribute)) is StaticIdAttribute attr) ? null : attr.Id);
        }

        internal static string GetName(this Type type)
        {
            var tableAttr = type?.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
            var nameAttr = type?.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            string name = (string.IsNullOrEmpty(tableAttr?.Name) ? nameAttr?.DisplayName : tableAttr?.Name);
            if (string.IsNullOrEmpty(name))
            {
                int genericTypeDelimeter = type.Name.IndexOf('`');
                name = (genericTypeDelimeter > 0 ? type.Name.Substring(genericTypeDelimeter) : type.Name);
            }

            return name;
        }

        internal static string GetName(this MemberInfo member)
        {
            var columnAttr = member?.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            var nameAttr = member?.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return (string.IsNullOrEmpty(columnAttr?.Name) ? nameAttr?.DisplayName : columnAttr?.Name) ?? member.Name;
        }

        internal static string GetIdOrName(this Table table)
        {
            return (!string.IsNullOrEmpty(table?.Id) ? $"$({table.Id})" : table.Name);
        }

        internal static string GetIdOrName(this Column column)
        {
            return (!string.IsNullOrEmpty(column.Id) ? $"$({column.Id})" : column.Name);
        }

        internal static string GetIdOrName(this MemberInfo member)
        {
            var suidAttr = member.GetCustomAttribute(typeof(StaticIdAttribute)) as StaticIdAttribute;

            if (!string.IsNullOrEmpty(suidAttr?.Id)) return $"$({suidAttr.Id})";
            else return GetName(member);
        }

        internal static string GetEnumName(this MemberInfo member)
        {
            var enumAttr = member.GetCustomAttribute(typeof(EnumValueAttribute)) as EnumValueAttribute;
            var nameAttr = member.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return (string.IsNullOrEmpty(enumAttr?.Name) ? nameAttr?.DisplayName : enumAttr?.Name) ?? member.Name;
        }

        internal static string GetVersion(this Assembly assembly)
        {
            Version version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        /// <summary>
        /// Escapes the specified sql object's name according to the rules of the specified <see cref="Language"/>.
        /// </summary>
        /// <param name="name">The sql object name.</param>
        /// <param name="syntax">The syntax.</param>
        /// <returns>An escaped name.</returns>
        internal static string Escape(this string name, Language syntax)
        {
            switch (syntax)
            {
                default:
                case Language.SQL:
                    return name;

                case Language.MySQL:
                    return $"`{name}`";

                case Language.TSQL:
                case Language.SQLite:
                    return $"[{name}]";
            }
        }

        internal static string ToText(this Order order)
        {
            string output;

            switch (order)
            {
                default:
                case Order.ASC:
                    output = "ASC";
                    break;

                case Order.DESC:
                    output = "DESC";
                    break;
            }

            return output;
        }

        internal static string ToText(this ReferentialAction action)
        {
            string output;

            switch (action)
            {
                default:
                case ReferentialAction.NoAction:
                    output = "NO ACTION";
                    break;

                case ReferentialAction.Cascade:
                    output = "CASCADE";
                    break;

                case ReferentialAction.SetNull:
                    output = "SET NULL";
                    break;

                case ReferentialAction.SetDefault:
                    output = "SET DEFAULT";
                    break;

                case ReferentialAction.Restrict:
                    output = "RESTRICT";
                    break;
            }

            return output;
        }
    }
}