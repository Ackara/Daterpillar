using Acklann.Daterpillar.Writers;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Migration;
using System.IO;
using System.Management.Automation;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// <para type="synopsis">Generates a new migration script from two '.schema.xml' files.</para>
    /// </summary>
    [OutputType(typeof(FileInfo))]
    [Cmdlet(VerbsCommon.New, "MigrationScript", ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true)]
    public class NewMigrationScriptCmdlet : Cmdlet
    {
        /// <summary>
        /// <para type="description">The absolute-path of old/current '.schema.xml' file.</para>
        /// </summary>
        [Alias("o", "old")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 1)]
        public string OldSchemaFilePath { get; set; }

        [Alias("n", "new")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 2)]
        public string NewSchemaFilePath { get; set; }

        [Alias("d", "dest")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 3)]
        public string Destination { get; set; }

        [Alias("l", "lang")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 4)]
        public Syntax Language { get; set; }

        [Parameter]
        public SwitchParameter OmitDropStatements { get; set; }

        protected override void ProcessRecord()
        {
            Schema oldSchema = null, newSchema = null;
            string error, outputFile = Destination, description = "update_schema";

            if (!File.Exists(OldSchemaFilePath))
                oldSchema = new Schema();
            else if (!Schema.TryLoad(OldSchemaFilePath, out oldSchema, out error))
                throw new System.ArgumentException($"{error} at '{OldSchemaFilePath}'.");

            if (!Schema.TryLoad(NewSchemaFilePath, out newSchema, out error))
                throw new System.ArgumentException($"{error} at '{NewSchemaFilePath}'.");

            if (!Path.HasExtension(outputFile))
                outputFile = Path.Combine(outputFile, $"V{newSchema.Version}__{description}.{Language.ToString().ToLowerInvariant()}.sql");

            if (ShouldProcess(outputFile))
            {
                Helper.CreateDirectory(outputFile);
                using (var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream))
                {
                    var factory = new SqlWriterFactory();
                    var migrator = new Migrator();
                    var changes = migrator.GenerateMigrationScript(factory.CreateInstance(Language, writer), oldSchema, newSchema, OmitDropStatements.IsPresent);

                    foreach (var item in changes)
                        WriteVerbose($"[{item.Action}] {item.Value.GetName()}");

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
}