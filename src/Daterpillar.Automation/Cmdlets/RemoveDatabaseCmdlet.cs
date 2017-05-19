using Ackara.Daterpillar.Migration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Management.Automation;

namespace Ackara.Daterpillar.Cmdlets
{
    /// <summary>
    /// A powershell cmdlet that removes an existing database from a SQL server.
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsCommon.Remove, "Database", DefaultParameterSetName = defaultArgs)]
    public class RemoveDatabaseCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the server host.
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
        /// Gets or sets the password.
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
        [Alias("c", "conn", "connStr")]
        [Parameter(ValueFromPipeline = true, ParameterSetName = defaultArgs)]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            BuildConnectionString(out string connectionString);
            IServerManager server = ServerManagerFactory.CreateInstance(Syntax, connectionString);
            bool databaseWasDropped = server.DropDatabase(Database);
            WriteVerbose($"Dropped the [{Database}] database from the '{GetHost(connectionString)}' server.");
            WriteObject(databaseWasDropped);
        }

        #region Private Members

        private const string defaultArgs = "default", explictArgs = "explict";

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
                    { "password", Password }
                };
                connectionString = builder.ConnectionString;
            }
            else { connectionString = ConnectionString; }
        }

        #endregion Private Members
    }
}