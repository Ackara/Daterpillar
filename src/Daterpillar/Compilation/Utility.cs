namespace Acklann.Daterpillar.Compilation
{
    internal static class Utility
    {
        public static string Escape(this string text) => text?.Replace("'", @"\'");

        public static string WithSpace(this string text) => string.IsNullOrEmpty(text) ? string.Empty : $" {text.Trim()}";
    }
}