using System;
using System.Configuration;
using System.Data;
using System.IO;

namespace MSTest.Daterpillar
{
    public static class ConnectionFactory
    {
        public static string GetMSSQLConnectionString(string database = "")
        {
            return (new System.Data.SqlClient.SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["mssql"].ConnectionString) { InitialCatalog = database }.ToString());
        }

        public static string GetMySQLConnectionString(string database = "")
        {
            return (new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString) { Database = database }.ToString());
        }

        public static string GetSQLiteConnectionString(string filename = null)
        {
            string pathToSaveDb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (filename ?? "emptyDb.db3"));
            if (string.IsNullOrEmpty(filename))
            {
                File.Delete(pathToSaveDb);
                System.Data.SQLite.SQLiteConnection.CreateFile(pathToSaveDb);
            }
            return (new System.Data.SQLite.SQLiteConnectionStringBuilder() { DataSource = pathToSaveDb }.ToString());
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
            return new System.Data.SQLite.SQLiteConnection(GetSQLiteConnectionString(filename));
        }

        public static bool TryExecuteScript(this IDbConnection connection, string script, out string errorMsg)
        {
            throw new System.NotImplementedException();
        }
    }
}