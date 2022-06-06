using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Daterpillar.Tool.Commands
{
    [Verb("export")]
    public class ExportCommand
    {
        [Option('p', "project", Required = true)]
        public string ProjectFile { get; set; }

        [Option('t', "type", Required = true)]
        public string EntryType { get; set; }

        [Option('o', "output")]
        public string OutputFilePath { get; set; }

        public void Execute()
        {
            if (!File.Exists(ProjectFile)) throw new FileNotFoundException($"Could not find file at '{ProjectFile}'.");
            if (string.IsNullOrEmpty(EntryType)) throw new ArgumentNullException(nameof(EntryType));

            string generatorProgramPath = BuildExportProject(ProjectFile);
            CallExportProjectProgram(generatorProgramPath);
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

            Assembly assembly = typeof(ExportCommand).Assembly;

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
    }
}