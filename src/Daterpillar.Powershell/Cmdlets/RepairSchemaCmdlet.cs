using System;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Repairs the schema history table.</para>
    /// <para type="description">This cmdlet Repairs the schema history table.</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// <para type="link">https://flywaydb.org/documentation/command/repair</para>
    /// </summary>
    /// <example>
    /// <code>
    /// Repair-DaterpillarSchema SQLite 'C:/app/db.sqlite'
    /// </code>
    /// <para>This will repair the schema history table.</para>
    /// </example>
    /// <seealso cref="FlywayCmdletWrapper"/>
    [Cmdlet(VerbsDiagnostic.Repair, (nameof(Daterpillar) + "Schema"), ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true)]
    public class RepairSchemaCmdlet : FlywayCmdletWrapper
    {
        /// <summary>Processes the record.</summary>
        /// <exception cref="Exception"></exception>
        protected override void ProcessRecord()
        {
            if (ShouldProcess(MigrationsDirectory, "flyway-repair"))
            {
                ProcessResult flyway = Flyway.Invoke("repair", ConnectionType, ConnectionString, MigrationsDirectory, FlywayFilePath, Timeout);
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