using Acklann.Daterpillar.Configuration;
using System.Collections;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
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
            return list.Count > 0;
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