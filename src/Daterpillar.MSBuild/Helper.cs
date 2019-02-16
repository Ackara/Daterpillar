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

        public static void Error(this IBuildEngine BuildEngine, string message)
        {
            BuildEngine.LogErrorEvent(new BuildErrorEventArgs(null, null, null, 0, 0, 0, 0, $"{nameof(Daterpillar)} | {message}", null, nameof(ExportSchemaTask)));
        }

        public static void Warn(this IBuildEngine engine, string message, string sender = nameof(ExportSchemaTask))
        {
            engine.LogWarningEvent(new BuildWarningEventArgs(null, null, null, 0, 0, 0, 0, $"{nameof(Daterpillar)} | {message}", null, sender));
        }

        public static void Info(this IBuildEngine BuildEngine, MessageImportance level, string message)
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(
                $"{nameof(Daterpillar)} | {message}",
                string.Empty,
                nameof(ExportSchemaTask),
                level));
        }
    }
}