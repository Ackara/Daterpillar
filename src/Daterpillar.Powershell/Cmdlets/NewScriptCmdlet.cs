using Acklann.Daterpillar.Attributes;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Migration;
using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Generates a new migration script from the two specified '.schema.xml' files.</para>
    /// <para type="description">This cmdlet creates a new migration script from two '.schema.xml' files.</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// </summary>
    [OutputType(typeof(PSObject))]
    [Cmdlet(VerbsCommon.New, (nameof(Daterpillar) + "MigrationScript"), ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true, DefaultParameterSetName = DEFAULT_SET)]
    public class NewScriptCmdlet : Cmdlet
    {
        private const string DEFAULT_SET = "default", PIPELINE_SET = "pipeline";
        private string _newSchemaFilePath = null;

        /// <summary>
        /// <para type="description">The absolute-path of old/production '.schema.xml' file.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("o", "old", "snapshot")]
        [Parameter(ParameterSetName = DEFAULT_SET, Mandatory = true, Position = 3)]
        [Parameter(ParameterSetName = PIPELINE_SET, Mandatory = true, Position = 3)]
        public string OldSchemaFilePath { get; set; }

        /// <summary>
        /// <para type="description">The absolute-path of new/current '.schema.xml' file.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("n", "new", "path", "fullname")]
        [Parameter(ParameterSetName = DEFAULT_SET, Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 4)]
        [Parameter(ParameterSetName = PIPELINE_SET, ValueFromPipelineByPropertyName = true, Position = 4)]
        public string NewSchemaFilePath
        {
            get { return (string.IsNullOrEmpty(_newSchemaFilePath) ? InputObject : _newSchemaFilePath); }
            set { _newSchemaFilePath = value; }
        }

        /// <summary>
        /// <para type="description">The absolute-path of the new migration script.</para>
        /// </summary>
        [Alias("d", "dest")]
        [Parameter(ParameterSetName = DEFAULT_SET, Position = 2)]
        [Parameter(ParameterSetName = PIPELINE_SET, Position = 2)]
        public string Destination { get; set; }

        /// <summary>
        /// <para type="description">The dialect of the sql script.</para>
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Alias("l", "lang")]
        [Parameter(ParameterSetName = DEFAULT_SET, Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = PIPELINE_SET, Mandatory = true, Position = 1)]
        public Language Language { get; set; }

        /// <summary>
        /// <para type="description">Exclude all drop statements, when present.</para>
        /// </summary>
        [Alias("no-drop")]
        [Parameter(ParameterSetName = DEFAULT_SET)]
        [Parameter(ParameterSetName = PIPELINE_SET)]
        public SwitchParameter OmitDropStatements { get; set; }

        /// <summary>
        /// Gets or sets the descripiton.
        /// </summary>
        /// <value>
        /// The descripiton.
        /// </value>
        [Alias("desc")]
        [Parameter(ParameterSetName = DEFAULT_SET)]
        [Parameter(ParameterSetName = PIPELINE_SET)]
        public string Descripiton { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = PIPELINE_SET, ValueFromPipeline = true)]
        public string InputObject { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        protected override void ProcessRecord()
        {
            Schema oldSchema = null, newSchema = null;
            string error, outputFile = null;

            if (!File.Exists(OldSchemaFilePath))
                oldSchema = new Schema();
            else if (!Schema.TryLoad(OldSchemaFilePath, out oldSchema, out error))
                throw new ArgumentException($"{error} at '{OldSchemaFilePath}'.");

            if (!Schema.TryLoad(NewSchemaFilePath, out newSchema, out error))
                throw new ArgumentException($"{error} at '{NewSchemaFilePath}'.");

            outputFile = Environment.ExpandEnvironmentVariables(string.Format((Destination ?? string.Empty), newSchema.Version, Descripiton, Language, (Language.ToString().ToLowerInvariant())));
            if (string.IsNullOrEmpty(outputFile))
                outputFile = Path.GetDirectoryName(OldSchemaFilePath);

            if (!Path.HasExtension(outputFile)) // not a file
                outputFile = Path.Combine(outputFile, $"V{newSchema.Version}__{Descripiton}.{Language.ToString().ToLowerInvariant()}.sql");

            if (ShouldProcess(oldSchema.ResolveName() ?? newSchema.ResolveName()))
            {
                PSHelper.EnsureDirectoryExists(outputFile);
                Discrepancy[] changes = (new Migrator()).GenerateMigrationScript(Language, oldSchema, newSchema, outputFile, OmitDropStatements.IsPresent);

                var builer = new StringBuilder();
                if (changes.Length > 0)
                {
                    foreach (Discrepancy item in changes)
                    {
                        WriteVerbose($" * {item.Action} {item.Value} {item.Value.GetType().Name}".ToLowerInvariant());
                        AppendDiscrepancy(builer, item);
                    }
                }
                else WriteVerbose("no changes detected.");

                WriteObject(new
                {
                    Warning = builer.ToString(),
                    Script = new FileInfo(outputFile),
                    OldSchema = new FileInfo(OldSchemaFilePath),
                    NewSchema = new FileInfo(NewSchemaFilePath)
                });
            }
        }

        private static void AppendDiscrepancy(StringBuilder builder, Discrepancy discrepancy)
        {
            bool accept(Discrepancy d) => d.Action != SqlAction.Create && (d.OldValue.GetType().IsDefined(typeof(TableAttribute)) || d.OldValue.GetType().IsDefined(typeof(ColumnAttribute)));

            if (accept(discrepancy))
                builder.AppendLine($"The '{discrepancy.OldValue.GetName()}' table was {discrepancy.Action}.");

            foreach (var item in discrepancy.Children)
                if (accept(item))
                {
                    builder.AppendLine($"\t[{item.OldValue.GetName()}].[{item.OldValue.GetName()}] column was {item.Action}.");
                }
        }
    }
}