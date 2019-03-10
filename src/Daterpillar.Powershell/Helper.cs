using Acklann.Daterpillar.Configuration;
using System;
using System.IO;

namespace Acklann.Daterpillar
{
    internal static class Helper
    {
        public static void CreateDirectory(string filePath)
        {
            string folder = Path.GetDirectoryName(filePath);
            if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
        }

        public static string Expand(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            else return System.Environment.ExpandEnvironmentVariables(value).Trim();
        }

        public static string GetTidyMessage(this Exception ex)
        {
            if (ex == null) return string.Empty;
            else return ex.Message.Replace(Schema.XMLNS, $"{nameof(Daterpillar)}xsd".ToLowerInvariant());
        }
    }
}