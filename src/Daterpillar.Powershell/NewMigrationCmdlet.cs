using Acklann.Daterpillar.Compilation;
using Acklann.Daterpillar.Configuration;
using System.IO;
using System.Management.Automation;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// This acutlly worked foo bar.
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [OutputType(typeof(FileInfo))]
    [Cmdlet(VerbsCommon.New, "MigrationScript", ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true)]
    public class NewMigrationCmdlet : Cmdlet
    {
        [Alias("o", "from", "old")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 1)]
        public string OldSchemaFilePath { get; set; }

        [ValidateNotNullOrEmpty]
        [Alias("n", "to", "new")]
        [Parameter(Mandatory = true, Position = 2)]
        public string NewSchemaFilePath { get; set; }

        [ValidateNotNullOrEmpty]
        [Alias("d", "path", "dest")]
        [Parameter(Mandatory = true, Position = 3)]
        public string Destination { get; set; }

        [ValidateNotNullOrEmpty]
        [Alias("l", "lang", "syntax")]
        [Parameter(Mandatory = true, Position = 4)]
        public Syntax Language { get; set; }

        [Parameter]
        public SwitchParameter OmitDropStatements { get; set; }

        protected override void ProcessRecord()
        {
            string error, outputFile = Destination;
            SchemaDeclaration oldSchema = null, newSchema = null;

            if (!File.Exists(OldSchemaFilePath))
                oldSchema = new SchemaDeclaration();
            else if (!SchemaDeclaration.TryLoad(OldSchemaFilePath, out oldSchema, out error))
                throw new System.ArgumentException(error);

            if (!SchemaDeclaration.TryLoad(NewSchemaFilePath, out newSchema, out error))
                throw new System.ArgumentException(error);

            if (!Path.HasExtension(outputFile))
                outputFile = Path.Combine(outputFile, $"V{newSchema.Version}__alter_schema.{Language.ToString().ToLowerInvariant()}.sql");

            if (ShouldProcess(outputFile))
            {
                Helper.CreateDirectory(outputFile);
                using (var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream))
                {
                    var factory = new SqlWriterFactory();
                    var migrator = new SqlMigrator();
                    var changes = migrator.GenerateMigrationScript(factory.CreateInstance(Language, writer), oldSchema, newSchema, OmitDropStatements.IsPresent);

                    foreach (var item in changes)
                        WriteVerbose($"[{item.Action}] {item.Value.GetName()}");

                    WriteObject(new FileInfo(outputFile));
                }
            }
        }
    }
}