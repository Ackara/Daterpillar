using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Acklann.Daterpillar.Migration
{
    /// <summary>
    /// Provides methods and properties to manage a SQLite database.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Migration.IServerManager" />
    public class SQLiteServerManager : IServerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteServerManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SQLiteServerManager(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
            _connStr = new SQLiteConnectionStringBuilder(connectionString);
        }

        /// <summary>
        /// Gets the full path to the SQLite database file.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath
        {
            get { return _connStr.DataSource; }
        }

        /// <summary>
        /// Creates a new database if it do not exist.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database was created, <c>false</c> otherwise.</returns>
        public bool CreateDatabase(string databaseName)
        {
            if (File.Exists(_connStr.DataSource) == false)
            {
                SQLiteConnection.CreateFile(_connStr.DataSource);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drops an existing database.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database existed, <c>false</c> otherwise.</returns>
        public bool DropDatabase(string databaseName)
        {
            if (File.Exists(_connStr.DataSource))
            {
                File.Delete(_connStr.DataSource);
                return true;
            }
            return false;
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
            return new SQLiteConnection(_connStr.ConnectionString);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _connection?.Dispose();
        }

        #region Private Member

        private readonly IDbConnection _connection;
        private readonly SQLiteConnectionStringBuilder _connStr;

        private void OpenConnectionIfClosed()
        {
            if (_connection.State != ConnectionState.Open) _connection.Open();
        }

        #endregion Private Member
    }
}