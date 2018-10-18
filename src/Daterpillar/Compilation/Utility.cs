using Acklann.Daterpillar.Configuration;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Acklann.Daterpillar.Compilation
{
    internal static class Utility
    {
        internal static string Escape(this string text) => text?.Replace("'", @"\'");

        internal static string WithSpace(this string text) => string.IsNullOrEmpty(text) ? string.Empty : $" {text.Trim()}";

        internal static bool IsIdentical(this Table left, Table right)
        {
            return
                (left.Id == right?.Id && left.Id != 0 && right?.Id != 0)
                ||
                string.Equals(left?.Name, right?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        internal static bool IsIdentical(this Column left, Column right)
        {
            return
                (left.Id == right?.Id && left.Id != 0 && right?.Id != 0)
                ||
                string.Equals(left.Name, right?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        internal static bool IsIdentical(this ISQLObject left, ISQLObject right)
        {
            if (left is Table)
                return IsIdentical((Table)left, (Table)right);
            else if (left is Column)
                return IsIdentical((Column)left, (Column)right);
            else
                return string.Equals(left.GetName(), right.GetName(), System.StringComparison.OrdinalIgnoreCase);
        }

        internal static int GetId(this MemberInfo member)
        {
            return (!(member.GetCustomAttribute(typeof(SUIDAttribute)) is SUIDAttribute attr) ? 0 : attr.Id);
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
            return (table.Id > 0 ? $"$({table.Id})" : table.Name);
        }

        internal static string GetIdOrName(this Column column)
        {
            return (column.Id > 0 ? $"$({column.Id})" : column.Name);
        }

        internal static string GetIdOrName(this MemberInfo member)
        {
            var suidAttr = member.GetCustomAttribute(typeof(SUIDAttribute)) as SUIDAttribute;

            if (suidAttr?.Id > 0) return $"$({suidAttr.Id})";
            else return GetName(member);
        }

        internal static string GetEnumName(this MemberInfo member)
        {
            var enumAttr = member.GetCustomAttribute(typeof(EnumValueAttribute)) as EnumValueAttribute;
            var nameAttr = member.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return (string.IsNullOrEmpty(enumAttr?.Name) ? nameAttr?.DisplayName : enumAttr?.Name) ?? member.Name;
        }
    }
}