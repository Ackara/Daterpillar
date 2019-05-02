using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Prints the details and status information about all the migrations.</para>
    /// <para type="description">This cmdlet prints the details and status information about all the migrations.</para>
    /// <para type="link">https://github.com/Ackara/Daterpillar</para>
    /// <para type="link">https://flywaydb.org/documentation/commandline/info</para>
    /// </summary>
    /// <seealso cref="FlywayCmdletWrapper" />
    [Cmdlet(VerbsCommon.Show, (nameof(Daterpillar) + "MigrationHistory"))]
    public class ShowHistoryCmdlet : FlywayCmdletWrapper
    {
        /// <summary>
        /// <para type="description"></para>
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            ProcessResult flyway = Flyway.Invoke("info", ConnectionType, ConnectionString, MigrationsDirectory, FlywayFilePath, Timeout);
            if (flyway.ExitCode != 0) throw new Exception(string.Concat(flyway.GetOutput()));

            if (PassThru)
            {
                string version = null;
                Match match = Regex.Match(flyway.StandardOutput, @"schema version: +(?<version>\d\.*)", RegexOptions.IgnoreCase);
                if (match.Success) version = match.Groups["version"]?.Value?.Trim();

                foreach (string line in flyway.GetOutput())
                    WriteVerbose(line);

                PSObject result = CreateInputObject();
                result.Members.Add(new PSNoteProperty("CurrentVersion", version));
                result.Members.Add(new PSNoteProperty("Output", flyway.StandardOutput));
                result.Members.Add(new PSNoteProperty("History", ParseTable(flyway.StandardOutput)));
                WriteObject(result);
                return;
            }

            foreach (string line in flyway.GetOutput())
                Console.WriteLine(line);
        }

        private PSObject[] ParseTable(string text)
        {
            bool firstRow = true;
            string[] headers = null;
            var results = new List<PSObject>();
            var pattern = new Regex(@"\| +[^\|]+ +");
            string getValue(Match m) => m.Value.Trim('|', ' ').Trim();

            foreach (string line in text.Split(new string[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                MatchCollection rows = pattern.Matches(line);
                if (rows.Count > 0)
                {
                    var item = new PSObject();

                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (firstRow)
                        {
                            if (headers == null) headers = new string[rows.Count];
                            headers[i] = getValue(rows[i]);
                            continue;
                        }

                        item.Members.Add(new PSNoteProperty(headers[i], getValue(rows[i])));
                    }

                    if (!firstRow) results.Add(item);
                    firstRow = false;
                }
            }

            return results.ToArray();
        }
    }
}