using System;

namespace Ackara.Daterpillar.Migration
{
    public static class ExtensionMethods
    {
        internal static string ToOrder(this string value)
        {
            if (value.StartsWith("asc", StringComparison.CurrentCultureIgnoreCase))
                return nameof(Order.Ascending);
            else if (value.StartsWith("desc", StringComparison.CurrentCultureIgnoreCase))
                return nameof(Order.Descending);
            else
                return value;
        }

        internal static IndexType ToIndexType(this string value)
        {
            foreach (var name in Enum.GetNames(typeof(IndexType)))
                if (string.Equals(name, value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return (IndexType)Enum.Parse(typeof(IndexType), name);
                }

            throw new ArgumentException($"[IndexType] do not contain the value '{value}'.", nameof(value));
        }

        internal static ReferentialAction ToReferentialAction(this string value)
        {
            value = value.ToLower().ToPascalCase();
            return (ReferentialAction)Enum.Parse(typeof(ReferentialAction), value);
        }
    }
}