using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Conversion;
using Acklann.GlobN;
using Microsoft.Build.Framework;
using System.IO;
using System.Xml.Schema;

namespace Acklann.Daterpillar
{
    public class ExportSchemaTask : ITask
    {
        [Required]
        public string AssemblyFile { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        public bool Execute()
        {
            if (File.Exists(AssemblyFile))
            {
                SchemaDeclaration schema = AssemblyConverter.ToSchema(AssemblyFile);
                string outFile = Path.ChangeExtension(AssemblyFile, ".schema.xml");
                Helper.CreateDirectory(outFile);

                if (string.IsNullOrEmpty(schema.Import) == false)
                {
                    Glob pattern = schema.Import;
                    foreach (var folder in new string[] { Path.GetDirectoryName(AssemblyFile), ProjectDirectory })
                    {
                        bool didNotFindDependency = true;
                        if (Directory.Exists(folder))
                            foreach (var filePath in pattern.ResolvePath(folder, SearchOption.TopDirectoryOnly, true))
                            {
                                ValidationEventHandler handler = delegate (object sender, ValidationEventArgs e)
                                {
                                    switch (e.Severity)
                                    {
                                        case XmlSeverityType.Error:
                                            BuildEngine.Error($"[{e.Severity}] {e.Message} at '{filePath}'");
                                            break;

                                        case XmlSeverityType.Warning:
                                            BuildEngine.Warn($"[{e.Severity}] {e.Message} at '{filePath}'");
                                            break;
                                    }
                                };

                                if (SchemaDeclaration.TryLoad(filePath, out SchemaDeclaration dependency, handler))
                                {
                                    schema.Merge(dependency);
                                    didNotFindDependency = false;
                                }
                            }

                        if (didNotFindDependency) BuildEngine.Warn($"Could not find '{schema.Import}'.");
                    }
                }

                using (var stream = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    schema.WriteTo(stream);
                    BuildEngine.Info(MessageImportance.High, $"Created '{outFile}'.");
                }

                return true;
            }
            else BuildEngine.Warn($"Could not find assembly file at '{AssemblyFile}'.");
            return false;
        }

        #region ITask

        public ITaskHost HostObject { get; set; }

        public IBuildEngine BuildEngine { get; set; }

        #endregion ITask
    }
}