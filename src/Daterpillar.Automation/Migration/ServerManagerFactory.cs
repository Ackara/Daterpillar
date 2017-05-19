using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    ///  Provide methods to create a new <see cref="IServerManager"/> instance.
    /// </summary>
    public class ServerManagerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerManagerFactory"/> class.
        /// </summary>
        public ServerManagerFactory()
        {
            _serverManagerTypes = new Dictionary<string, Type>()
            {
                { nameof(Syntax.MSSQL).ToLower(), typeof(MSSQLServerManager) },
                { nameof(System.Data.SqlClient.SqlConnection).ToLower(), typeof(MSSQLServerManager) },

                { nameof(Syntax.MySQL).ToLower(), typeof(MySQLServerManager) },
                { nameof(MySql.Data.MySqlClient.MySqlConnection).ToLower(), typeof(MySQLServerManager) },

                { nameof(Syntax.SQLite).ToLower(), typeof(SQLiteServerManager) },
                { nameof(System.Data.SQLite.SQLiteConnection).ToLower(), typeof(SQLiteServerManager) },
            };

            var serverTypes = from type in Assembly.GetAssembly(typeof(IServerManager)).GetExportedTypes()
                              where typeof(IServerManager).IsAssignableFrom(type) && type.IsAbstract == false && type.IsInterface == false
                              select type;

            foreach (var type in serverTypes)
            {
                _serverManagerTypes.Add(type.Name.ToLower(), type);
            }
        }

        /// <summary>
        /// Creates a new <see cref="IServerManager"/> instance that matches the specified name.
        /// </summary>
        /// <param name="name">The name of the object type to create.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IServerManager.</returns>
        public static IServerManager CreateInstance(string name, string connectionString)
        {
            if (_instance == null) _instance = new ServerManagerFactory();
            return _instance.Create(name, connectionString);
        }

        /// <summary>
        /// Creates a new <see cref="IServerManager"/> instance that matches the specified name.
        /// </summary>
        /// <param name="name">The name of the object type to create.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IServerManager.</returns>
        public IServerManager Create(string name, string connectionString)
        {
            try
            {
                Type type = _serverManagerTypes[name.ToLower()];
                return (IServerManager)Activator.CreateInstance(type, connectionString);
            }
            catch (KeyNotFoundException)
            {
                return new NullServerManager();
            }
        }

        /// <summary>
        /// Creates a new <see cref="IServerManager"/> instance that is associated with the specified syntax.
        /// </summary>
        /// <param name="syntax">The syntax.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IServerManager.</returns>
        public IServerManager Create(Syntax syntax, string connectionString)
        {
            return Create(syntax.ToString(), connectionString);
        }

        #region Private Members

        private static ServerManagerFactory _instance;
        private IDictionary<string, Type> _serverManagerTypes;

        #endregion Private Members
    }
}