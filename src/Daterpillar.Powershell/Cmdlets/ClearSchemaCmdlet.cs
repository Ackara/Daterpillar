using System;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Drops all objects (tables, views, procedures, triggers, etc.).</para>
    /// <para type="description">This cmdlet drops all objects (tables, views, procedures, triggers, etc.).</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// <para type="link">https://flywaydb.org/documentation/commandline/clean</para>
    /// </summary>
    /// <example>
    /// <code>Clear-DaterpillarSchema SQLite 'C:/app/db.sqlite'</code>
    /// <para>This will remove all table from the schema.</para>
    /// </example>
    /// <seealso cref="FlywayCmdletWrapper" />
    [Cmdlet(VerbsCommon.Clear, (nameof(Daterpillar) + "Schema"), ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true)]
    public class ClearSchemaCmdlet : FlywayCmdletWrapper
    {
        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void ProcessRecord()
        {
            if (ShouldProcess(MigrationsDirectory, "flyway-clean"))
            {
                ProcessResult flyway = Flyway.Invoke("clean", ConnectionType, ConnectionString, MigrationsDirectory, FlywayFilePath, Timeout);
                if (flyway.ExitCode != 0) throw new Exception(string.Concat(flyway.GetOutput()));

                foreach (string line in flyway.GetOutput())
                    WriteVerbose(line);

                PSObject result = CreateInputObject();
                result.Members.Add(new PSNoteProperty("Output", flyway.StandardOutput));
                WriteObject(result);
            }
        }
    }
}