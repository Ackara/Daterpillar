using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Conversion;
using Acklann.GlobN;
using System.IO;
using System.Management.Automation;
using System.Xml.Schema;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// <para type="synposis">Generates a '.schema.xml' file from a '.dll' file.</para>
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsData.Export, "Schema", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true)]
    public class ExportSchemaCmdlet : Cmdlet
    {
        /// <summary>
        /// <para type="description">The absolute-path of your project.</para>
        /// </summary>
        /// <value>
        /// The project directory.
        /// </value>
        [Parameter(Position = 1)]
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Gets or sets the assembly file.
        /// <para type="description">The absolute-path of the target assembly.</para>
        /// </summary>
        /// <value>
        /// The assembly file.
        /// </value>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 2)]
        public string AssemblyFile { get; set; }

        protected override void ProcessRecord()
        {
            if (File.Exists(AssemblyFile))
            {
                SchemaDeclaration schema = AssemblyConverter.ToSchema(AssemblyFile);
                string outputFile = Path.ChangeExtension(AssemblyFile, ".schema.xml");
                Helper.CreateDirectory(outputFile);

                if (string.IsNullOrEmpty(ProjectDirectory)) { ProjectDirectory = Directory.GetCurrentDirectory(); }

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
                                            throw new System.Xml.XmlException($"[{e.Severity}] {e.Message} at '{filePath}'");

                                        case XmlSeverityType.Warning:
                                            WriteWarning($"[{e.Severity}] {e.Message} at '{filePath}'");
                                            break;
                                    }
                                };

                                if (SchemaDeclaration.TryLoad(filePath, out SchemaDeclaration dependency, handler))
                                {
                                    schema.Merge(dependency);
                                    didNotFindDependency = false;
                                }
                            }

                        if (didNotFindDependency) WriteWarning($"Could not find '{schema.Import}'.");
                    }
                }

                using (var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    schema.WriteTo(stream);
                }
                WriteVerbose($"Created '{outputFile}'.");
                WriteObject(new FileInfo(outputFile));
            }
            else throw new FileNotFoundException($"Could not find assembly file at '{AssemblyFile}'.");
        }
    }
}