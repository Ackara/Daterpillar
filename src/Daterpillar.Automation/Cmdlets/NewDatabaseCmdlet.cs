﻿using Acklann.Daterpillar.Migration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// A powershell cmdlet that attaches a new database to a SQL server.
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsCommon.New, "Database")]
    public class NewDatabaseCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the server host.
        /// </summary>
        /// <value>The host.</value>
        [Alias("h", "server")]
        [Parameter(Position = 0)]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user.</value>
        [Alias("u", "usr")]
        [Parameter(Position = 1)]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Alias("p", "pwd")]
        [Parameter(Position = 2)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        [Alias("d", "name")]
        [Parameter(Position = 3)]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the syntax.
        /// </summary>
        /// <value>The syntax.</value>
        [Parameter(Position = 4, Mandatory = true)]
        [Alias("type", "syn", "ext", "extension")]
        [ValidateSet(nameof(Daterpillar.Syntax.MSSQL), nameof(Daterpillar.Syntax.MySQL), nameof(Daterpillar.Syntax.SQLite))]
        public string Syntax { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        [Alias("conn", "connStr")]
        [Parameter(ValueFromPipeline = true)]
        public string ConnectionString { get; set; }

        [Parameter]
        [Alias("r", "del", "dlt", "delete")]
        public SwitchParameter DeleteIfExist { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            BuildConnectionString(out string connectionString);
            IServerManager server = ServerManagerFactory.CreateInstance(Syntax, connectionString);
            bool databaseWasCreated = server.CreateDatabase(Database);
            if (DeleteIfExist.IsPresent && databaseWasCreated == false)
            {
                server.DropDatabase(Database);
                databaseWasCreated = server.CreateDatabase(Database);
            }
            WriteVerbose(databaseWasCreated ? $"Attached the [{Database}] database to the '{GetHost(connectionString)}' server." : $"The [{Database}] database already exist.");
            WriteObject(databaseWasCreated);
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