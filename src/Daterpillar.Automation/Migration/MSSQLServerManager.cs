using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Provide methods and properties for managing a MSSQL server.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Migration.IServerManager" />
    public class MSSQLServerManager : IServerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSQLServerManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MSSQLServerManager(string connectionString)
        {
            _connectionString = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            _server = new Server(_connectionString.DataSource);
            _server.ConnectionContext.LoginSecure = false;
            _server.ConnectionContext.Login = _connectionString.UserID;
            _server.ConnectionContext.Password = _connectionString.Password;
        }

        /// <summary>
        /// Creates a new database if it do not exist.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database was created, <c>false</c> otherwise.</returns>
        public bool CreateDatabase(string databaseName)
        {
            if (_server.Databases[databaseName] == null)
            {
                new Database(_server, databaseName).Create();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Drops an existing database.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database existed, <c>false</c> otherwise.</returns>
        public bool DropDatabase(string databaseName)
        {
            if (_server.Databases[databaseName] == null) { return false; }
            else
            {
                _server.KillAllProcesses(databaseName);
                _server.KillDatabase(databaseName);
                return true;
            }
        }

        /// <summary>
        /// Creates a new <see cref="IDbConnection" /> instance.
        /// </summary>
        /// <returns>A new <see cref="IDbConnection" /> instance.</returns>
        public IDbConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(_connectionString.ConnectionString);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #region Private Members

        private readonly Server _server;
        private System.Data.SqlClient.SqlConnectionStringBuilder _connectionString;

        #endregion Private Members
    }
}