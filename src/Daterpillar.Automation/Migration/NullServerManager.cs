using System.Data;

namespace Acklann.Daterpillar.Migration
{
    /// <summary>
    /// Represents a null <see cref="IServerManager"/>. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Migration.IServerManager" />
    public sealed class NullServerManager : IServerManager
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c>.</returns>
        public bool CreateDatabase(string databaseName)
        {
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns><c>true</c>.</returns>
        public bool DropDatabase(string databaseName)
        {
            return true;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns><c>null</c></returns>
        public IDbConnection GetConnection()
        {
            return null;
        }
    }
}