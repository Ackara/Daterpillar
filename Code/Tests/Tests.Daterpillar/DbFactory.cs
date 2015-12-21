using System;
using System.Data.SQLite;
using System.IO;


namespace Tests.Daterpillar
{
    public static class DbFactory
    {
        public static object TextContext { get; private set; }

        public static SQLiteConnection CreateSQLiteConnection(string schema = null)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp.db");
            if (File.Exists(dbPath)) File.Delete(dbPath);

            string connectionString = new SQLiteConnectionStringBuilder() { DataSource = dbPath }.ConnectionString;
            var connection = new SQLiteConnection(connectionString);

            if (schema != null)
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = schema;
                    command.ExecuteNonQuery();
                }
            }

            return connection;
        }
    }
}