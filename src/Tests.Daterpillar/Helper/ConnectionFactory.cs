using System;
using System.Data;
using System.IO;

namespace Tests.Daterpillar.Helper
{
    public static class ConnectionFactory
    {
        public static IDbConnection CreateMySQLConnection(string database = null)
        {
            var connStr = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder("");
            if (!string.IsNullOrEmpty(database)) connStr.Database = database;

            return new MySql.Data.MySqlClient.MySqlConnection(connStr.ConnectionString);
        }

        public static IDbConnection CreateMSSQLConnection(string database = null)
        {
            var connStr = new System.Data.SqlClient.SqlConnectionStringBuilder("");
            if (!string.IsNullOrEmpty(database)) connStr.Add("database", database);

            return new System.Data.SqlClient.SqlConnection(connStr.ConnectionString);
        }

        public static IDbConnection CreateSQLiteConnection(string filePath = "")
        {
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "daterpillar-test.db3");
                System.Data.SQLite.SQLiteConnection.CreateFile(filePath);
            }

            var connStr = new System.Data.SQLite.SQLiteConnectionStringBuilder() { DataSource = filePath };
            return new System.Data.SQLite.SQLiteConnection(connStr.ConnectionString);
        }
    }
}