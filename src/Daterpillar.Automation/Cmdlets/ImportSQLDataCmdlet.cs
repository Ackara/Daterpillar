using Acklann.Daterpillar.Migration;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Text;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// A powershell cmdlet that imports data from one SQL table to another on the same or different server. This class cannot be inherited.
    /// </summary>
    /// <remarks>Both the source and target tables must be identical.</remarks>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsData.Import, "SQLData")]
    public sealed class ImportSQLDataCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the connection to the source database.
        /// </summary>
        /// <value>The source.</value>
        [Alias("src", "from")]
        [Parameter(Position = 0, Mandatory = true)]
        public object Source { get; set; }

        /// <summary>
        /// Gets or sets the connection to the destination.
        /// </summary>
        /// <value>The destination.</value>
        [Alias("d", "dest", "to", "tgt")]
        [Parameter(Position = 1, Mandatory = true)]
        public object Destination { get; set; }

        /// <summary>
        /// Gets or sets the SQL table.
        /// </summary>
        /// <value>The table.</value>
        [Alias("t", "tbl")]
        [Parameter(Position = 2, Mandatory = true)]
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the syntax.
        /// </summary>
        /// <value>The syntax.</value>
        [Alias("s")]
        [Parameter(Position = 3)]
        [ValidateSet(nameof(Daterpillar.Syntax.MSSQL), nameof(Daterpillar.Syntax.MySQL), nameof(Daterpillar.Syntax.SQLite))]
        public string Syntax { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            DataTable data;
            IDbConnection source = GetConnection(Source);
            string sourceHostName = GetHost(source.ConnectionString);
            using (source)
            {
                source.Open();
                WriteVerbose($"connected to the '{sourceHostName}' database.");

                using (var command = source.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM {Table};";
                    using (data = new DataTable())
                    {
                        WriteVerbose($"querying the '{sourceHostName}' database ...");
                        data.Load(command.ExecuteReader());
                        WriteVerbose($"query completed; {data.Rows.Count} records were retrieved.");
                    }
                }
            }

            if (data.Rows.Count == 0) { WriteWarning($"The [{Table}] table do not have any records."); }
            else
            {
                IDbConnection destination = GetConnection(Destination);
                string destinationHostName = GetHost(destination.ConnectionString);
                using (destination)
                {
                    destination.Open();
                    WriteVerbose($"connected to the '{destinationHostName}' database.");

                    using (var command = destination.CreateCommand())
                    {
                        WriteVerbose($"importing {data.Rows.Count} records to the '{destinationHostName}' database ...");
                        string batchInsert = BuildInsertScript(data);
                        command.CommandText = batchInsert;
                        command.ExecuteNonQuery();
                        WriteVerbose($"import complete.");
                    }
                }
            }

            WriteObject(data);
        }

        #region Private Members

        private IDbConnection GetConnection(object input)
        {
            if (input is PSObject ps) input = ps.BaseObject;

            if (input is IDbConnection connection)
            {
                return connection;
            }
            else
            {
                return ServerManagerFactory.CreateInstance(Syntax, input.ToString()).GetConnection();
            }
        }

        private string BuildInsertScript(DataTable data)
        {
            using (data)
            {
                var script = new StringBuilder();
                script.Append($"INSERT INTO {data.TableName} (");

                foreach (DataColumn column in data.Columns)
                {
                    script.Append($"{column.ColumnName}, ");
                }

                script.Remove((script.Length - 2), 2);
                script.AppendLine($") VALUES");
                foreach (DataRow record in data.Rows)
                {
                    script.Append("(");
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        script.Append($"'{record[i]}', ");
                    }
                    script.Remove((script.Length - 2), 2);
                    script.AppendLine("),");
                }

                script.Remove((script.Length - 3), 3);
                script.Append(";");

                return script.ToString();
            }
        }

        private string GetHost(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            string databaseName = (string.IsNullOrEmpty(builder.InitialCatalog) ? "master" : builder.InitialCatalog);
            return $"{builder.DataSource}/{databaseName}";
        }

        #endregion Private Members
    }
}