using Acklann.Daterpillar.Configuration;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Acklann.Daterpillar.Writers
{
    internal static class Utility
    {
        internal static string Escape(this string text) => text?.Replace("'", @"\'");

        internal static string WithSpace(this string text) => string.IsNullOrEmpty(text) ? string.Empty : $" {text.Trim()}";

        internal static bool IsIdentical(this TableDeclaration left, TableDeclaration right)
        {
            return
                (left.Id == right?.Id && !string.IsNullOrEmpty(left?.Id) && !string.IsNullOrEmpty(right?.Id))
                ||
                string.Equals(left?.Name, right?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        internal static void CreateDirectory(string scriptFile)
        {
            if (string.IsNullOrEmpty(scriptFile)) throw new ArgumentNullException(nameof(scriptFile));

            string folder = Path.GetDirectoryName(scriptFile);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }

        internal static bool IsIdentical(this ColumnDeclaration left, ColumnDeclaration right)
        {
            return
                (left.Id == right?.Id && !string.IsNullOrEmpty(left?.Id) && !string.IsNullOrEmpty(right?.Id))
                ||
                string.Equals(left.Name, right?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        internal static bool IsIdentical(this ISqlStatement left, ISqlStatement right)
        {
            if (left is TableDeclaration)
                return IsIdentical((TableDeclaration)left, (TableDeclaration)right);
            else if (left is ColumnDeclaration)
                return IsIdentical((ColumnDeclaration)left, (ColumnDeclaration)right);
            else
                return string.Equals(left.GetName(), right.GetName(), System.StringComparison.OrdinalIgnoreCase);
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

        internal static string GetIdOrName(this TableDeclaration table)
        {
            return (!string.IsNullOrEmpty(table?.Id) ? $"$({table.Id})" : table.Name);
        }

        internal static string GetIdOrName(this ColumnDeclaration column)
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
    }
}