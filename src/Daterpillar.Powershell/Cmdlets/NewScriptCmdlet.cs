using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Migration;
using System.IO;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Generates a new migration script from the two specified '.schema.xml' files.</para>
    /// <para type="description">This cmdlet creates a new migration script from two '.schema.xml' files.</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// </summary>
    [OutputType(typeof(PSObject))]
    [Cmdlet(VerbsCommon.New, (nameof(Daterpillar) + "MigrationScript"), ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true)]
    public class NewScriptCmdlet : Cmdlet
    {
        /// <summary>
        /// <para type="description">The absolute-path of old/production '.schema.xml' file.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("o", "old", "snapshot"), Parameter(Mandatory = true, Position = 3)]
        public string OldSchemaFilePath { get; set; }

        /// <summary>
        /// <para type="description">The absolute-path of new/current '.schema.xml' file.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("n", "new"), Parameter(Mandatory = true, ValueFromPipeline = true, Position = 4)]
        public string NewSchemaFilePath { get; set; }

        /// <summary>
        /// <para type="description">The absolute-path of the new migration script.</para>
        /// </summary>
        [Alias("d", "dest"), Parameter(Position = 2)]
        public string Destination { get; set; }

        /// <summary>
        /// <para type="description">The dialect of the sql script.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("l", "lang"), Parameter(Mandatory = true, Position = 1)]
        public Language Language { get; set; }

        /// <summary>
        /// <para type="description">Exclude all drop statements, when present.</para>
        /// </summary>
        [Alias("no-drop"), Parameter]
        public SwitchParameter OmitDropStatements { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        protected override void ProcessRecord()
        {
            Schema oldSchema = null, newSchema = null;
            string error, outputFile = Destination, description = "schema_update";

            if (!File.Exists(OldSchemaFilePath))
                oldSchema = new Schema();
            else if (!Schema.TryLoad(OldSchemaFilePath, out oldSchema, out error))
                throw new System.ArgumentException($"{error} at '{OldSchemaFilePath}'.");

            if (!Schema.TryLoad(NewSchemaFilePath, out newSchema, out error))
                throw new System.ArgumentException($"{error} at '{NewSchemaFilePath}'.");

            if (string.IsNullOrEmpty(outputFile))
                outputFile = Path.GetDirectoryName(OldSchemaFilePath);

            if (!Path.HasExtension(outputFile))
                outputFile = Path.Combine(outputFile, $"V{newSchema.Version}__{description}.{Language.ToString().ToLowerInvariant()}.sql");

            if (ShouldProcess(outputFile))
            {
                Helper.CreateDirectory(outputFile);
                var changes = (new Migrator()).GenerateMigrationScript(outputFile, oldSchema, newSchema, Language, OmitDropStatements.IsPresent);

                if (changes.Length > 0)
                {
                    WriteVerbose("changes detected:");
                    foreach (var item in changes)
                        WriteVerbose($"[{item.Action}] {item.Value.GetName()}");
                }
                else WriteVerbose("no changes detected.");

                WriteObject(new
                {
                    Script = new FileInfo(outputFile),
                    OldSchema = new FileInfo(OldSchemaFilePath),
                    NewSchema = new FileInfo(NewSchemaFilePath)
                });
            }
        }
    }
}