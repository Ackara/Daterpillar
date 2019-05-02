using System;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Migrates a schema to the latest version.</para>
    /// <para type="description">This cmdlet will execute all pending migration scripts in the specified directory.</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// <para type="link">https://flywaydb.org/documentation/commandline/migrate</para>
    /// </summary>
    /// <seealso cref="Cmdlet" />
    [Cmdlet(VerbsData.Update, (nameof(Daterpillar) + "Schema"), ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true)]
    public class UpdateSchemaCmdlet : FlywayCmdletWrapper
    {
        /// <summary>
        /// <para type="description">The current/production '.schema.xml' file. Typically the 'snapshot.schema.xml' file.</para>
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public FileInfo OldSchema { get; set; }

        /// <summary>
        /// <para type="description">The new '.schema.xml' file. Typically the '[assembly].schema.xml' file.</para>
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public FileInfo NewSchema { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void ProcessRecord()
        {
            ProcessResult flyway = new ProcessResult();
            if (Directory.Exists(MigrationsDirectory))
            {
                if (ShouldProcess(MigrationsDirectory, "flyway-migrate"))
                {
                    flyway = Flyway.Invoke("migrate", ConnectionType, ConnectionString, MigrationsDirectory, FlywayFilePath, Timeout);
                    if (flyway.ExitCode != 0) throw new Exception(string.Concat(flyway.GetOutput()));

                    if (OldSchema != null && NewSchema != null)
                    {
                        NewSchema.CopyTo(OldSchema.FullName, overwrite: true);
                        WriteVerbose($"replaced '{OldSchema.Name}' with '{NewSchema.Name}'.");
                    }

                    foreach (string line in flyway.GetOutput())
                        if (!string.IsNullOrWhiteSpace(line))
                            WriteVerbose(line.Trim());
                }
            }
            else WriteWarning($"Could not find any migration-scripts at '{MigrationsDirectory}'.");

            PSObject result = CreateInputObject();
            var (count, time) = ParseOutput(flyway.StandardOutput);

            result.Members.Add(new PSNoteProperty("Success", true));
            result.Members.Add(new PSNoteProperty("ExecutionTime", time));
            result.Members.Add(new PSNoteProperty("TotalMigrations", count));
            WriteObject(result);
        }

        private (int, string) ParseOutput(string output)
        {
            int count = 0; TimeSpan time = default(TimeSpan);

            if (!string.IsNullOrEmpty(output))
            {
                Match match = Regex.Match(output, @"(?i)successfully applied (?<count>\d+) migration to schema .+ \(execution time (?<time>\d+:\d+\.\d+)\w+\)");
                if (match.Success)
                {
                    int.TryParse(match.Groups["count"].Value, out count);
                    TimeSpan.TryParse($"00:{match.Groups["time"].Value}", out time);
                }
            }

            return (count, time.ToString());
        }
    }
}