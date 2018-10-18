using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Compilation
{
    internal static class Utility
    {
        public static string Escape(this string text) => text?.Replace("'", @"\'");

        public static string WithSpace(this string text) => string.IsNullOrEmpty(text) ? string.Empty : $" {text.Trim()}";

        public static bool IsIdentical(this Table left, Table right)
        {
            return
                (left.Id == right?.Id && left.Id != 0 && right?.Id != 0)
                ||
                string.Equals(left?.Name, right?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        public static bool IsIdentical(this Column left, Column right)
        {
            return
                (left.Id == right?.Id && left.Id != 0 && right?.Id != 0)
                ||
                string.Equals(left.Name, right?.Name, System.StringComparison.OrdinalIgnoreCase)
                ;
        }

        public static bool IsIdentical(this ISQLObject left, ISQLObject right)
        {
            if (left is Table)
                return IsIdentical((Table)left, (Table)right);
            else if (left is Column)
                return IsIdentical((Column)left, (Column)right);
            else
                return false;
        }
    }
}