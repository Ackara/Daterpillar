using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;

namespace Tests.Daterpillar
{
    public static class DbFactory
    {
        public static SQLiteConnection CreateSQLiteConnection(string schema = null)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"temp{DateTime.Now.ToString("HHmmssfff")}.db");
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

        public static MySqlConnection CreateMySqlConnection(string database = "mysql")
        {
            string connectionString = ConfigurationManager.ConnectionStrings[database].ConnectionString;
            return new MySqlConnection(connectionString);
        }

        public static MySqlConnection CreateMySqlConnection(Schema schema)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                var settings = new MySqlTemplateSettings()
                {
                    CommentsEnabled = false,
                    DropDataIfExist = true
                };

                command.CommandText = new MySqlTemplate(settings).Transform(schema);
                command.ExecuteNonQuery();
            }

            return connection;
        }
    }
}