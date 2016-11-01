using Gigobyte.Daterpillar;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace Tests.Daterpillar.Helpers
{
    public static class DatabaseHelper
    {
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

        public static IDbConnection CreateMySQLConnection(string database = "")
        {
            var connStr = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(ConnectionString.GetMySQLServerConnectionString());
            if (!string.IsNullOrEmpty(database)) connStr.Database = database;

            return new MySql.Data.MySqlClient.MySqlConnection(connStr.ConnectionString);
        }

        public static IDbConnection CreateMSSQLConnection(string database = "")
        {
            var connStr = new System.Data.SqlClient.SqlConnectionStringBuilder(ConnectionString.GetSQLServerConnectionString());
            if (!string.IsNullOrEmpty(database)) connStr.Add("database", database);

            return new System.Data.SqlClient.SqlConnection(connStr.ConnectionString);
        }
        
        public static void DropDatabase(this IDbConnection connection, string database)
        {
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DROP DATABASE {database};";
                    command.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public static bool TryTruncateDatabase(this IDbConnection connection, Schema schema, bool dispose = true)
        {
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                using (var command = connection.CreateCommand())
                {
                    for (int i = schema.Tables.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            command.CommandText = $"DROP TABLE {schema.Tables[i].Name};";
                            command.ExecuteNonQuery();
                        }
                        catch (DbException) { }
                    }
                }

                return true;
            }
            catch (DbException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            finally { if (dispose) connection.Dispose(); }
        }
    }
}