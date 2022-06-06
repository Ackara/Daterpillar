using Microsoft.Build.Framework;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Acklann.Daterpillar.Targets
{
    public class ExportDatabaseSchema : ITask
    {
        [Required]
        public ITaskItem ProjectFile { get; set; }

        [Required]
        public string EntryType { get; set; }

        public string OutputFilePath { get; set; }

        public bool Execute()
        {
            string projectFilePath = ProjectFile.GetMetadata("FullPath");
            if (string.IsNullOrWhiteSpace(OutputFilePath)) OutputFilePath = Path.Combine(Path.GetDirectoryName(projectFilePath), "obj", $"{Path.GetFileNameWithoutExtension(projectFilePath)}.schema.xml");
            Message($"In-{nameof(ProjectFile)}: {projectFilePath}");
            Message($"In-{nameof(OutputFilePath)}: {OutputFilePath}");

            string generatorProgramPath = BuildExportProject(projectFilePath);
            CallExportProjectProgram(generatorProgramPath);
            return true;
        }

        private void CallExportProjectProgram(string projectFile)
        {
            if (!File.Exists(projectFile)) throw new FileNotFoundException($"Could not find file at '{projectFile}'.");

            var args = new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(projectFile),
                FileName = "dotnet",
                Arguments = string.Format("run --project \"{0}\"", projectFile),
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (var exe = new Process() { StartInfo = args })
            {
                exe.Start();
                exe.WaitForExit(30 * 1000);
            }
        }

        private string BuildExportProject(string targetProject)
        {
            if (!File.Exists(targetProject)) throw new FileNotFoundException($"Could not find file at '{targetProject}'.");

            // STEP: Create working directory

            string projectName = Path.GetFileNameWithoutExtension(targetProject);
            string workingDirectory = Path.Combine(Path.GetTempPath(), nameof(Daterpillar), projectName);
            if (Directory.Exists(workingDirectory)) Directory.Delete(workingDirectory, true);
            Directory.CreateDirectory(workingDirectory);

            // STEP: Export project files

            Assembly assembly = typeof(ExportDatabaseSchema).Assembly;

            void export(string resourceName, string outPath, params object[] args)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    string text = reader.ReadToEnd();
                    text = string.Format(text, args);
                    File.WriteAllText(outPath, text);
                }
            }
            string projectFile = Path.Combine(workingDirectory, $"{projectName}.Schema.csproj");
            export($"{assembly.GetName().Name}.project.xml", projectFile, targetProject);
            export($"{assembly.GetName().Name}.program.txt", Path.Combine(workingDirectory, $"Program.cs"), EntryType, OutputFilePath);

            return projectFile;
        }

        #region Backing Members

        public ITaskHost HostObject { get; set; }

        public IBuildEngine BuildEngine { get; set; }

        private void Message(string message, MessageImportance importance = MessageImportance.Normal)
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(message, null, nameof(Daterpillar), importance));
        }

        #endregion Backing Members
    }
}