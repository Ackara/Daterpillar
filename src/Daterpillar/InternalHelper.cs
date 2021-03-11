using Acklann.Daterpillar.Attributes;
using Acklann.Daterpillar.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar
{
    internal static class InternalHelper
    {
        public static IEnumerable<PropertyInfo> GetColumns(this Type model)
        {
            return from t in model.GetProperties()
                   where t.IsDefined(typeof(Attributes.ColumnAttribute))
                   select t;
        }

        internal static string Escape(this string text) => text?.Replace("'", @"\'");

        /// <summary>
        /// Determines whether the specified list is empty.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if the specified list is empty; otherwise, <c>false</c>.</returns>
        internal static bool IsEmpty(this ICollection list)
        {
            return list.Count == 0;
        }

        /// <summary>
        /// Determines whether the specified list is not empty.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if the specified list is not empty; otherwise, <c>false</c>.</returns>
        internal static bool IsNotEmpty(this ICollection list)
        {
            return list?.Count > 0;
        }

        internal static string WithSpace(this string text) => string.IsNullOrEmpty(text) ? string.Empty : $" {text.Trim()}";

        internal static void EnsureDirectoryExists(string filePath)
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

        internal static bool IsIdentical(this ISchemaObject x, ISchemaObject y)
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

        internal static string GetIdOrName(this Type type)
        {
            var suidAttr = type.GetCustomAttribute(typeof(StaticIdAttribute)) as StaticIdAttribute;

            if (!string.IsNullOrEmpty(suidAttr?.Id)) return $"$({suidAttr.Id})";
            else return GetName(type);
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