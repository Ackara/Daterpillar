using Acklann.Daterpillar.Migration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// A powershell cmdlet that executes a SQL command. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsLifecycle.Invoke, "SQLCommand")]
    public sealed class InvokeSqlCommandCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        [Alias("h", "server")]
        [Parameter(Position = 2)]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user.</value>
        [Alias("u", "usr")]
        [Parameter(Position = 3)]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        /// <value>The password.</value>
        [Alias("p", "pwd")]
        [Parameter(Position = 4)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        [Alias("d", "name")]
        [Parameter(Position = 5)]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the syntax.
        /// </summary>
        /// <value>The syntax.</value>
        [Parameter(Position = 1, Mandatory = true)]
        [Alias("type", "syn", "ext", "extension")]
        [ValidateSet(nameof(Daterpillar.Syntax.MSSQL), nameof(Daterpillar.Syntax.MySQL), nameof(Daterpillar.Syntax.SQLite))]
        public string Syntax { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        [Parameter]
        [Alias("conn", "connStr")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        /// <value>The script.</value>
        [Alias("content")]
        [Parameter(Position = 6, Mandatory = true, ValueFromPipeline = true)]
        public string Script { get; set; }

        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            string connectionString = BuildConnectionString();
            _server = ServerManagerFactory.CreateInstance(Syntax, connectionString);
        }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            int rowsAffected = _server.ExecuteNonQuery(Script);
            int limit = (Script.Length > 25 ? 25 : Script.Length);
            WriteVerbose($"executed command '{Script.Substring(0, limit)} ...' successfully.");
            WriteObject(true);
        }

        /// <summary>
        /// Ends the processing.
        /// </summary>
        protected override void EndProcessing()
        {
            _server?.Dispose();
        }

        #region Private Members

        private const string defaultArgs = "default", explictArgs = "explict";
        private IServerManager _server;

        private string GetHost(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.DataSource;
        }

        private string BuildConnectionString()
        {
            string connectionString = "";
            if (string.IsNullOrEmpty(ConnectionString))
            {
                string dataSource = Path.IsPathRooted(Host) ? "Data Source" : "server";
                var builder = new DbConnectionStringBuilder
                {
                    { dataSource, Host },
                    { "user", User },
                    { "password", Password },
                    { "database", Database }
                };
                connectionString = builder.ConnectionString;
            }
            else { connectionString = ConnectionString; }

            return connectionString;
        }

        #endregion Private Members
    }
}