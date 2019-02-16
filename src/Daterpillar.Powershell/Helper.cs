using System.IO;

namespace Acklann.Daterpillar
{
    internal static class Helper
    {
        public static string CreateDirectory(string filePath)
        {
            string folder = Path.GetDirectoryName(filePath);
            if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
            return filePath;
        }
    }
}