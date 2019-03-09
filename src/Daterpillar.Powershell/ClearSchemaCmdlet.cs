using System;
using System.Management.Automation;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// <para type="synopsis">Drops all objects (tables, views, procedures, triggers, etc.).</para>
    /// <para type="description">This cmdlet drops all objects (tables, views, procedures, triggers, etc.).</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// <para type="link">https://flywaydb.org/documentation/commandline/clean</para>
    /// </summary>
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
            ProcessResult flyway = Flyway.Invoke("clean", Language, ConnectionString, MigrationsDirectory, FlywayFilePath, Timeout);
            if (flyway.ExitCode != 0) throw new Exception(string.Concat(flyway.GetOutput()));

            foreach (string line in flyway.GetOutput())
                WriteVerbose(line);

            PSObject result = CreateInputObject();
            result.Members.Add(new PSNoteProperty("Output", flyway.StandardOutput));
            WriteObject(result);
        }
    }
}