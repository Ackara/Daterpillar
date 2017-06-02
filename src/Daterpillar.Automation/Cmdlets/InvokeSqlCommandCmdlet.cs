using Acklann.Daterpillar.Migration;
using System.Data;
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
    [Cmdlet(VerbsLifecycle.Invoke, "SQLCommand", DefaultParameterSetName = defaultArgs)]
    public sealed class InvokeSqlCommandCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        [Alias("h", "server")]
        [Parameter(Position = 0)]
        [Parameter(ParameterSetName = explictArgs)]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user.</value>
        [Alias("u", "usr")]
        [Parameter(Position = 1)]
        [Parameter(ParameterSetName = explictArgs)]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        /// <value>The password.</value>
        [Alias("p", "pwd")]
        [Parameter(Position = 2)]
        [Parameter(ParameterSetName = explictArgs)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        [Alias("d")]
        [Parameter(Position = 3)]
        [Parameter(ParameterSetName = defaultArgs)]
        [Parameter(ParameterSetName = explictArgs)]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the syntax.
        /// </summary>
        /// <value>The syntax.</value>
        [Alias("s")]
        [Parameter(Position = 4, Mandatory = true)]
        [Parameter(ParameterSetName = defaultArgs)]
        [Parameter(ParameterSetName = explictArgs)]
        [ValidateSet(nameof(Daterpillar.Syntax.MSSQL), nameof(Daterpillar.Syntax.MySQL), nameof(Daterpillar.Syntax.SQLite))]
        public string Syntax { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        [Alias("c", "connStr")]
        [Parameter(ParameterSetName = defaultArgs)]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        /// <value>The script.</value>
        [Parameter(Position = 5, Mandatory = true)]
        [Parameter(ParameterSetName = defaultArgs, ValueFromPipeline = true)]
        public string Script { get; set; }

        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            BuildConnectionString(out string connectionString);
            IServerManager server = ServerManagerFactory.CreateInstance(Syntax, connectionString);
            _connection = server.GetConnection();
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                var builder = new SqlConnectionStringBuilder(_connection.ConnectionString);
                WriteVerbose($"connected to '{builder.DataSource}' server.");
            }
        }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = Script;
                command.ExecuteNonQuery();

                int limit = (Script.Length > 25 ? 25 : Script.Length);
                WriteVerbose($"executed command '{Script.Substring(0, limit)} ...' successfully.");
                WriteObject(true);
            }
        }

        /// <summary>
        /// Ends the processing.
        /// </summary>
        protected override void EndProcessing()
        {
            _connection?.Dispose();
        }

        #region Private Members

        private const string defaultArgs = "default", explictArgs = "explict";
        private IDbConnection _connection;

        private string GetHost(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.DataSource;
        }

        private void BuildConnectionString(out string connectionString)
        {
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
        }

        #endregion Private Members
    }
}