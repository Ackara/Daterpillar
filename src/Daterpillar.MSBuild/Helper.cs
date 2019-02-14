using Microsoft.Build.Framework;
using System.IO;

namespace Acklann.Daterpillar
{
    internal static class Helper
    {
        public static string CreateDirectory(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
            return filePath;
        }

        public static void WriteError(this IBuildEngine BuildEngine, string message)
        {
            BuildEngine.LogErrorEvent(new BuildErrorEventArgs(null, null, null, 0, 0, 0, 0, message, null, nameof(GenerateMigrationScriptTask)));
        }

        public static void Write(this IBuildEngine BuildEngine, MessageImportance level, string message)
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(
                message,
                string.Empty,
                nameof(GenerateMigrationScriptTask),
                level));
        }
    }
}