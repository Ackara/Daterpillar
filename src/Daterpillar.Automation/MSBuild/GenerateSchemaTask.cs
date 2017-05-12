using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Reflection;

namespace Ackara.Daterpillar.MSBuild
{
    /// <summary>
    /// Represents a msbuild task that serializes a .dll file to a schema.
    /// </summary>
    /// <seealso cref="Microsoft.Build.Framework.ITask" />
    public class GenerateSchemaTask : ITask
    {
        /// <summary>
        /// Gets or sets the build engine associated with the task.
        /// </summary>
        /// <value>The build engine.</value>
        public IBuildEngine BuildEngine { get; set; }

        /// <summary>
        /// Gets or sets any host object that is associated with the task.
        /// </summary>
        /// <value>The host object.</value>
        public ITaskHost HostObject { get; set; }

        /// <summary>
        /// Gets or sets the assembly file.
        /// </summary>
        /// <value>The assembly file.</value>
        [Required]
        public string AssemblyFile { get; set; }

        /// <summary>
        /// Gets or sets the schema path.
        /// </summary>
        /// <value>The schema path.</value>
        [Output]
        public string SchemaPath { get; set; }

        /// <summary>
        /// Executes a task.
        /// </summary>
        /// <returns>true if the task executed successfully; otherwise, false.</returns>
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