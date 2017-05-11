using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Reflection;

namespace Ackara.Daterpillar.MSBuild
{
    public class GenerateSchemaTask : ITask
    {
        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }

        [Required]
        public string AssemblyFile { get; set; }

        [Output]
        public string SchemaPath { get; set; }

        public bool Execute()
        {
            AssemblyFile = Path.GetFullPath(Environment.ExpandEnvironmentVariables(AssemblyFile));
            if (File.Exists(AssemblyFile) == false)
            {
                BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"Cannot find the assembly file '{AssemblyFile}'.", "missing", GetType().Name, MessageImportance.High));
                return false;
            }

            var outputDir = Path.GetDirectoryName(AssemblyFile);
            var fileName = Path.GetFileNameWithoutExtension(AssemblyFile);
            SchemaPath = Path.Combine(outputDir, $"{fileName}.schema.xml");

            //BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"IN: {AssemblyFile} OUT: {SchemaPath}", "schema", nameof(GenerateSchemaTask), MessageImportance.High));

            Schema schema = Assembly.LoadFrom(AssemblyFile).ToSchema();
            using (var outStream = new FileStream(SchemaPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                schema.Save(outStream);
            }
            return true;
        }
    }
}