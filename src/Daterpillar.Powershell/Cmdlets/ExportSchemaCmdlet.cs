using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Migration;
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
    [Cmdlet(VerbsData.Export, (nameof(Daterpillar) + "Schema"), ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true, DefaultParameterSetName = "default")]
    public class ExportSchemaCmdlet : Cmdlet
    {
        /// <summary>
        /// <para type="description">The absolute-path of the target assembly.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("a", "path", "fullName")]
        [Parameter(ParameterSetName = "default", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public string AssemblyFile { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "piped", ValueFromPipeline = true)]
        public string InputObject { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <exception cref="FileNotFoundException">Could not find assembly file.</exception>
        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(AssemblyFile)) AssemblyFile = InputObject;

            if (File.Exists(AssemblyFile))
            {
                Schema schema = SchemaFactory.CreateFrom(AssemblyFile);
                string outputFile = Path.ChangeExtension(AssemblyFile, ".schema.xml");
                PSHelper.CreateDirectory(outputFile);
                schema.Merge();

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