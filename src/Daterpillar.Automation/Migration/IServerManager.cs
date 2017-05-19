﻿using System.Data;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Provides functionality to manager a SQL server.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IServerManager : System.IDisposable
    {
        /// <summary>
        /// Creates a new database if it do not exist.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database was created, <c>false</c> otherwise.</returns>
        bool CreateDatabase(string databaseName);

        /// <summary>
        /// Drops an existing database.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c> if the database existed, <c>false</c> otherwise.</returns>
        bool DropDatabase(string databaseName);

        /// <summary>
        /// Creates a new <see cref="IDbConnection"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IDbConnection"/> instance.</returns>
        IDbConnection GetConnection();
    }
}