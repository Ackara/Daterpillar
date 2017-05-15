using Microsoft.Build.Framework;
using System;
using System.IO;

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
            // Check if the assembly file exist
            AssemblyFile = Path.GetFullPath(Environment.ExpandEnvironmentVariables(AssemblyFile));
            if (File.Exists(AssemblyFile) == false)
            {
                BuildEngine?.LogMessageEvent(new BuildMessageEventArgs($"Cannot find the assembly file '{AssemblyFile}'.", "missing", GetType().Name, MessageImportance.High));
                return false;
            }

            Schema schema = null;
            SchemaPath = Path.ChangeExtension(AssemblyFile, "schema.xml");

            using (var outStream = new FileStream(SchemaPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                schema.Save(outStream);
                BuildEngine?.LogMessageEvent(new BuildMessageEventArgs($"Created '{SchemaPath}'.", "New", nameof(GenerateSchemaTask), MessageImportance.Normal));
            }
            return true;
        }
    }
}