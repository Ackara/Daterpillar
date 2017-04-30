using Ackara.Daterpillar;
using Ackara.Daterpillar.Scripting;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace MSTest.Daterpillar
{
    public static class ConnectionFactory
    {
        public static string GetMSSQLConnectionString(string database = "")
        {
            return (new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["mssql"].ConnectionString) { InitialCatalog = database }.ToString());
        }

        public static string GetMySQLConnectionString(string database = "")
        {
            return (new MySqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString) { Database = database }.ToString());
        }

        public static string GetSQLiteConnectionString(string filename = null)
        {
            filename = Path.HasExtension(filename) ? filename : $"{filename}.db3";
            string pathToSaveDb = Path.Combine(Path.GetTempPath(), (filename ?? "dtpl_emptyDb.db3"));
            if (File.Exists(pathToSaveDb) == false) SQLiteConnection.CreateFile(pathToSaveDb);
            return (new SQLiteConnectionStringBuilder() { DataSource = pathToSaveDb }.ToString());
        }

        public static IDbConnection CreateConnection(string syntax, string database = null)
        {
            return CreateConnection((Syntax)Enum.Parse(typeof(Syntax), syntax), database);
        }

        public static IDbConnection CreateConnection(Syntax syntax, string database = null)
        {
            IDbConnection connection;
            switch (syntax)
            {
                default:
                case Syntax.MSSQL:
                case Syntax.Generic:
                    connection = CreateMSSQLConnection(database);
                    break;

                case Syntax.MySQL:
                    connection = CreateMySQLConnection(database);
                    break;

                case Syntax.SQLite:
                    connection = CreateSQLiteConnection(database);
                    break;
            }

            return connection;
        }

        public static IDbConnection CreateMSSQLConnection(string database = "")
        {
            return new System.Data.SqlClient.SqlConnection(GetMSSQLConnectionString(database));
        }

        public static IDbConnection CreateMySQLConnection(string database = "")
        {
            return new MySql.Data.MySqlClient.MySqlConnection(GetMySQLConnectionString(database));
        }

        public static IDbConnection CreateSQLiteConnection(string filename = null)
        {
            return new SQLiteConnection(GetSQLiteConnectionString(filename));
        }

        public static void UseSchema(this IDbConnection connection, string relativePath)
        {
            string script = "";
            Schema schema = MockData.GetSchema(relativePath);
            schema.Name = connection.Database;

            if (connection is SqlConnection)
            {
                script = new MSSQLScriptBuilder().Append(schema).GetContent();
                RebuildMSSQLDatabase(connection);
            }
            else if (connection is MySqlConnection)
            {
                var builder = new MySQLScriptBuilder();
                builder.Append(schema);
                script = builder.GetContent();

                RebuildMySQLDatabase(connection);
            }
            else if (connection is SQLiteConnection sqliteConn)
            {
                var builder = new SQLiteScriptBuilder();
                builder.Append(schema);
                script = builder.GetContent();

                RebuildSQLiteDatabase(connection);
            }

            if (connection.State != ConnectionState.Open) connection.Open();
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = script;
                command.ExecuteNonQuery();
            }
        }

        public static void UseEmptyDatabase(this IDbConnection connection)
        {
            string databaseName = connection.Database;
            if (connection is SqlConnection mssqlConn)
            {
                RebuildMSSQLDatabase(connection);
            }
            else if (connection is MySqlConnection mysqlConn)
            {
                RebuildMySQLDatabase(connection);
            }
            else if (connection is SQLiteConnection sqliteConn)
            {
                RebuildSQLiteDatabase(connection);
            }
        }

        public static bool TryExecuteScript(this IDbConnection connection, string script, out string errorMsg)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            IDbCommand command = connection.CreateCommand();
            errorMsg = null;

            try
            {
                command.CommandText = script;
                command.ExecuteNonQuery();

                return true;
            }
            catch (System.Data.Common.DbException ex)
            {
                errorMsg = string.Format("{2}{0}{2}{2}{1}{2}", ex.Message, ex, Environment.NewLine);
                return false;
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine($"connection:  {connection.ConnectionString}");
                command?.Dispose();
            }
        }

        internal static void RebuildMSSQLDatabase(IDbConnection connection)
        {
            string databaseName = connection.Database;
            var server = connection.AsSqlServerInstance();

            var database = server.Databases[databaseName];
            if (database != null)
            {
                server.KillAllProcesses(databaseName);
                server.KillDatabase(databaseName);
            }
            new Microsoft.SqlServer.Management.Smo.Database(server, databaseName).Create();
            if (connection.State == ConnectionState.Open) connection.ChangeDatabase(databaseName);
        }

        internal static void RebuildMySQLDatabase(IDbConnection connection)
        {
            var connStr = new MySqlConnectionStringBuilder(connection.ConnectionString) { Database = null };
            using (var connection2 = new MySqlConnection(connStr.ConnectionString))
            {
                if (connection2.State != ConnectionState.Open) connection2.Open();
                using (IDbCommand command = connection2.CreateCommand())
                {
                    string databaseName = connection.Database;
                    command.CommandText = $"DROP DATABASE IF EXISTS `{databaseName}`;";
                    command.CommandText += $"CREATE DATABASE IF NOT EXISTS `{databaseName}`;";
                    command.ExecuteNonQuery();
                }
            }
        }

        internal static void RebuildSQLiteDatabase(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Open) connection.Close();

            var connStr = new SQLiteConnectionStringBuilder(connection.ConnectionString);
            string pathToSaveDb = connStr.DataSource;
            if (File.Exists(pathToSaveDb)) File.Delete(pathToSaveDb);
            SQLiteConnection.CreateFile(pathToSaveDb);
        }

        internal static Microsoft.SqlServer.Management.Smo.Server AsSqlServerInstance(this IDbConnection connection)
        {
            var connStr = new SqlConnectionStringBuilder(connection.ConnectionString);
            var server = new Microsoft.SqlServer.Management.Smo.Server(connStr.DataSource);
            server.ConnectionContext.LoginSecure = false;
            server.ConnectionContext.Login = connStr.UserID;
            server.ConnectionContext.Password = connStr.Password;

            return server;
        }
    }
}