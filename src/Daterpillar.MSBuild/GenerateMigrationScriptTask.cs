using Acklann.Daterpillar.Compilation;
using Microsoft.Build.Framework;
using System.IO;

namespace Acklann.Daterpillar
{
    public class GenerateMigrationScriptTask : ITask
    {
        [Required]
        public string AssemblyFile { get; set; }

        [Required]
        public string MigrationDirectory { get; set; }

        public bool Execute()
        {
            if (File.Exists(AssemblyFile) == false)
            {
                BuildEngine.WriteError($"Could not find assembly at '{AssemblyFile}'.");
                return false;
            }

            var schema = SchemaConvert.ToSchema(AssemblyFile);
            string outFile = Path.ChangeExtension(AssemblyFile, ".schema.xml");
            Helper.CreateDirectory(outFile);

            using (var stream = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                schema.Save(stream);
            }

            return true;
        }

        #region ITask

        public ITaskHost HostObject { get; set; }

        public IBuildEngine BuildEngine { get; set; }

        #endregion ITask
    }
}