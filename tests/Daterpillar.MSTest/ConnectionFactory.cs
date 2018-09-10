using Acklann.Daterpillar;
using Acklann.Daterpillar.Scripting;
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public static void UseEmptyDatabase(this IDbConnection connection)
        {
            throw new System.NotImplementedException();
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
                System.Diagnostics.Debug.WriteLine($"used connection: {connection.ConnectionString}");
                errorMsg = string.Format("{2}{0}{2}{2}{1}{2}", ex.Message, ex, Environment.NewLine);
                return false;
            }
            finally { command?.Dispose(); }
        }

        internal static void RebuildMSSQLDatabase(IDbConnection connection)
        {
            throw new System.NotImplementedException();
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

        internal static object AsSqlServerInstance(this IDbConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}