using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Migration;
using Acklann.GlobN;
using System;
using System.IO;
using System.Management.Automation;
using System.Xml.Schema;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Generates a '.schema.xml' file from a '.dll' file.</para>
    /// <para type="description">This cmdlet create a '.schema.xml' file from a '.dll' file.</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// <list type="alertSet">
    /// <item>
    /// <term>ProjectDirectory</term>
    /// <description>
    /// The project directory is solely used to locate any scripts specified in the '.shema.xml' import tag.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <seealso cref="Cmdlet" />
    [OutputType(typeof(FileInfo))]
    [Cmdlet(VerbsData.Export, (nameof(Daterpillar) + "Schema"), ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true)]
    public class ExportSchemaCmdlet : Cmdlet
    {
        /// <summary>
        /// <para type="description">The absolute-path of the target assembly.</para>
        /// </summary>
        [Alias("a", "path")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string AssemblyFile { get; set; }

        /// <summary>
        /// <para type="description">The absolute-path of your project.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("proj"), Parameter]
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <exception cref="FileNotFoundException">Could not find assembly file at '{AssemblyFile}</exception>
        protected override void ProcessRecord()
        {
            if (File.Exists(AssemblyFile))
            {
                Schema schema = SchemaFactory.CreateFrom(AssemblyFile);
                string outputFile = Path.ChangeExtension(AssemblyFile, ".schema.xml");
                Helper.CreateDirectory(outputFile);

                if (string.IsNullOrEmpty(schema.Import) == false)
                {
                    if (string.IsNullOrEmpty(ProjectDirectory)) { ProjectDirectory = Directory.GetCurrentDirectory(); }

                    Glob pattern = schema.Import;
                    foreach (var cwd in new string[] { Path.GetDirectoryName(AssemblyFile), ProjectDirectory })
                    {
                        bool didNotFindDependency = true;
                        if (Directory.Exists(cwd))
                            foreach (var filePath in pattern.ResolvePath(cwd, SearchOption.TopDirectoryOnly))
                                if (Schema.TryLoad(Environment.ExpandEnvironmentVariables(filePath), out Schema dependency, OnValidate))
                                {
                                    schema.Merge(dependency);
                                    didNotFindDependency = false;
                                    break;
                                }

                        if (didNotFindDependency) WriteWarning($"Could not find '{schema.Import}'.");
                    }
                }

                using (var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    schema.WriteTo(stream);
                    WriteVerbose($"created '{outputFile}'.");
                    WriteObject(new FileInfo(outputFile));
                }
            }
            else throw new FileNotFoundException($"Could not find assembly file at '{AssemblyFile}'.");
        }

        private void OnValidate(XmlSeverityType severity, XmlSchemaException ex)
        {
            string message = $"[{severity}] {ex.GetTidyMessage()} At Line:{ex.LineNumber} Column:{ex.LinePosition}.";
            switch (severity)
            {
                case XmlSeverityType.Error:
                    WriteError(new ErrorRecord(new System.Xml.XmlException(message), $"DTP{(int)severity:00}", ErrorCategory.SyntaxError, null));
                    break;

                case XmlSeverityType.Warning:
                    WriteWarning(message);
                    break;
            }
        }
    }
}