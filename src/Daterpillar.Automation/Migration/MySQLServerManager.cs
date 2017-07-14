using MySql.Data.MySqlClient;
using System.Data;

namespace Acklann.Daterpillar.Migration
{
    /// <summary>
    /// Provides methods and properties to manage a MySQL server instance.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Migration.IServerManager" />
    public class MySQLServerManager : IServerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLServerManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MySQLServerManager(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connStr = new MySqlConnectionStringBuilder(connectionString);
        }

        /// <summary>
        /// Creates a new database if it do not exist.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database was created, <c>false</c> otherwise.</returns>
        public bool CreateDatabase(string databaseName)
        {
            OpenConnectionIfClosed();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE `{databaseName}`;";
                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException ex) when (ex.Number == 1007) { return false; }
            }
        }

        /// <summary>
        /// Drops an existing database.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database existed, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool DropDatabase(string databaseName)
        {
            OpenConnectionIfClosed();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = $"DROP DATABASE IF EXISTS `{databaseName}`;";
                command.ExecuteNonQuery();
            }
            return true;
        }

        /// <summary>
        /// Executes a SQL statement against the underlying connection object.
        /// </summary>
        /// <param name="script">The SQL statement.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string script)
        {
            OpenConnectionIfClosed();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = script;
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a new <see cref="IDbConnection" /> instance.
        /// </summary>
        /// <returns>A new <see cref="IDbConnection" /> instance.</returns>
        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connStr.ConnectionString);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _connection?.Dispose();
        }

        #region Private Members

        private readonly IDbConnection _connection;
        private readonly MySqlConnectionStringBuilder _connStr;

        private void OpenConnectionIfClosed()
        {
            if (_connection.State != ConnectionState.Open) _connection.Open();
        }

        #endregion Private Members
    }
}