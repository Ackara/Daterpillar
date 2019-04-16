using Acklann.Daterpillar.Configuration;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Acklann.Daterpillar
{
    internal static class Helper
    {
        public static string Escape(this string text) => text?.Replace("'", @"\'");

        public static string WithSpace(this string text) => string.IsNullOrEmpty(text) ? string.Empty : $" {text.Trim()}";

        public static void CreateDirectory(string scriptFile)
        {
            if (string.IsNullOrEmpty(scriptFile)) throw new ArgumentNullException(nameof(scriptFile));

            string folder = Path.GetDirectoryName(scriptFile);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }

        public static bool IsIdentical(this Table x, Table y)
        {
            return
                (x.Id == y?.Id && !string.IsNullOrEmpty(x?.Id) && !string.IsNullOrEmpty(y?.Id))
                ||
                string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase)
                ;
        }

        public static bool IsIdentical(this Column x, Column y)
        {
            return
                (x.Id == y?.Id && !string.IsNullOrEmpty(x?.Id) && !string.IsNullOrEmpty(y?.Id))
                ||
                string.Equals(x.Name, y?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        public static bool IsIdentical(this Script x, Script y)
        {
            return
                string.Equals(x.Name, y?.Name, StringComparison.OrdinalIgnoreCase)
                &&
                x.Syntax == y.Syntax;
        }

        public static bool IsIdentical(this ISqlObject x, ISqlObject y)
        {
            if (x is Table)
                return IsIdentical((Table)x, (Table)y);
            else if (x is Column)
                return IsIdentical((Column)x, (Column)y);
            else
                return string.Equals(x.GetName(), y.GetName(), StringComparison.OrdinalIgnoreCase);
        }

        public static string GetId(this MemberInfo member)
        {
            return (!(member.GetCustomAttribute(typeof(StaticIdAttribute)) is StaticIdAttribute attr) ? null : attr.Id);
        }

        public static string GetName(this Type type)
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

        public static string GetName(this MemberInfo member)
        {
            var columnAttr = member?.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            var nameAttr = member?.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return (string.IsNullOrEmpty(columnAttr?.Name) ? nameAttr?.DisplayName : columnAttr?.Name) ?? member.Name;
        }

        public static string GetIdOrName(this Table table)
        {
            return (!string.IsNullOrEmpty(table?.Id) ? $"$({table.Id})" : table.Name);
        }

        public static string GetIdOrName(this Column column)
        {
            return (!string.IsNullOrEmpty(column.Id) ? $"$({column.Id})" : column.Name);
        }

        public static string GetIdOrName(this MemberInfo member)
        {
            var suidAttr = member.GetCustomAttribute(typeof(StaticIdAttribute)) as StaticIdAttribute;

            if (!string.IsNullOrEmpty(suidAttr?.Id)) return $"$({suidAttr.Id})";
            else return GetName(member);
        }

        public static string GetEnumName(this MemberInfo member)
        {
            var enumAttr = member.GetCustomAttribute(typeof(EnumValueAttribute)) as EnumValueAttribute;
            var nameAttr = member.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return (string.IsNullOrEmpty(enumAttr?.Name) ? nameAttr?.DisplayName : enumAttr?.Name) ?? member.Name;
        }

        public static string GetVersion(this Assembly assembly)
        {
            Version version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

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
        /// <param name="separator">The separator.</param>
        /// <returns>The specified string converted to pascal case.</returns>
        /// <remarks><list type="bullet">
        ///   <listheader>Rules</listheader>
        ///   <item>Capitalize the first letter in the text.</item>
        ///   <item>Capitalize the first letter for each subsequent word.</item>
        ///   <item>Concatenate each word.</item>
        /// </list></remarks>
        public static string ToPascalCase(this string text, params char[] separator)
        {
            if (text.Length == 1) return text.ToUpper();
            else
            {
                separator = (separator.Length == 0 ? new char[] { ' ', '_' } : separator);
                string[] words = text.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                string pascal = "";

                foreach (var word in words)
                {
                    pascal += char.ToUpper(word[0]) + word.Substring(1);
                }

                return pascal;
            }
        }

        /// <summary>
        /// Converts the specified string to camel case.
        /// </summary>
        /// <param name="text">The string to convert to camel case.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The specified string converted to camel case.</returns>
        /// <remarks><list type="bullet">
        ///   <listheader>Rules</listheader>
        ///   <item>Lower the first letter in the text.</item>
        ///   <item>Capitalize the first letter for each subsequent word.</item>
        ///   <item>Concatenate each word.</item>
        /// </list></remarks>
        public static string ToCamelCase(this string text, params char[] separator)
        {
            if (text.Length == 1) return text.ToLower();
            else
            {
                string pascal = ToPascalCase(text, separator);
                return char.ToLower(pascal[0]) + pascal.Substring(1);
            }
        }

        /// <summary>
        /// Escapes the specified sql object's name according to the rules of the specified <see cref="Language"/>.
        /// </summary>
        /// <param name="name">The sql object name.</param>
        /// <param name="syntax">The syntax.</param>
        /// <returns>An escaped name.</returns>
        public static string Escape(this string name, Language syntax)
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