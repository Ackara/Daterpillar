using System;
using System.Text;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Provides extension methods for string objects.
    /// </summary>
    public static class StringExtensions
    {
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
    }
}