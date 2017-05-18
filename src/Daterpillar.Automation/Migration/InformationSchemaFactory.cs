using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Provide methods to create a new <see cref="IInformationSchema"/> instance.
    /// </summary>
    public class InformationSchemaFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationSchemaFactory"/> class.
        /// </summary>
        public InformationSchemaFactory()
        {
            _infoSchemaTypes = new Dictionary<string, Type>()
            {
                { nameof(Syntax.MSSQL).ToLower(), typeof(MSSQLInformationSchema) },
                { nameof(System.Data.SqlClient.SqlConnection).ToLower(), typeof(System.Data.SqlClient.SqlConnection) },

                { nameof(Syntax.MySQL).ToLower(), typeof(MySQLInformationSchema) },
                { nameof(MySql.Data.MySqlClient.MySqlConnection).ToLower(), typeof(MySQLInformationSchema) },

                { nameof(Syntax.SQLite).ToLower(), typeof(SQLiteInformationSchema) },
                { nameof(System.Data.SQLite.SQLiteConnection).ToLower(), typeof(SQLiteInformationSchema) },
            };

            var infoTypes = from t in Assembly.GetAssembly(typeof(InformationSchemaFactory)).GetTypes()
                            where typeof(IInformationSchema).IsAssignableFrom(t) && t.IsInterface == false && t.IsAbstract == false
                            select t;

            foreach (var type in infoTypes)
            {
                _infoSchemaTypes.Add(type.Name.ToLower(), type);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IInformationSchema"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A <see cref="IInformationSchema"/> instance.</returns>
        public static IInformationSchema CreateInstance(IDbConnection connection)
        {
            if (_instance == null) _instance = new InformationSchemaFactory();
            return _instance.Create(connection, connection.GetType().Name);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IInformationSchema"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name of the type.</param>
        /// <returns>A <see cref="IInformationSchema"/> instance.</returns>
        public IInformationSchema Create(IDbConnection connection, string name)
        {
            try
            {
                Type type = _infoSchemaTypes[name.ToLower()];
                return (IInformationSchema)Activator.CreateInstance(type, connection);
            }
            catch (KeyNotFoundException)
            {
                return new NullInformationSchema();
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IInformationSchema"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="syntax">The syntax.</param>
        /// <returns>A <see cref="IInformationSchema"/> instance.</returns>
        public IInformationSchema Create(IDbConnection connection, Syntax syntax)
        {
            return Create(connection, syntax.ToString());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IInformationSchema"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A <see cref="IInformationSchema"/> instance.</returns>
        public IInformationSchema Create(IDbConnection connection)
        {
            return Create(connection, connection.GetType().Name);
        }

        #region Private Members

        private static InformationSchemaFactory _instance;
        private readonly IDictionary<string, Type> _infoSchemaTypes;

        #endregion Private Members
    }
}