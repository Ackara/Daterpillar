using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar
{
    public static class MockDatabase
    {
        public static IDbConnection CreateConnection(Language kind = Language.SQLite, [CallerMemberName]string name = null)
        {
            var config = JObject.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connections.json")));
            string connectionString = config.SelectToken((kind).ToString().ToLowerInvariant())?.Value<string>();

            switch (kind)
            {
                default:
                    return CreateSQLiteConnection(name);

                case Language.TSQL:
                    return CreateMSSSQLConnection(connectionString);

                case Language.MySQL:
                    return CreateMySQLConnection(connectionString);
            }
        }

        public static bool TryExecute(this IDbConnection connection, string sql, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                foreach (string batch in new Regex(@"(?i)\r?\nGO\r?\n").Split(sql))
                    if (string.IsNullOrEmpty(batch) == false)
                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.CommandText = batch;
                            command.ExecuteNonQuery();
                        }

                return true;
            }
            catch (Exception ex)
            {
                errorMsg = $@"
********** !!! TEST FAILED !!! **********
{ex.ToString()}
*****************************************

".TrimStart();
            }

            return false;
        }

        public static void RebuildSchema(this IDbConnection connection, string name, bool addDummyTables = true)
        {
            switch (connection)
            {
                default:
                case System.Data.SQLite.SQLiteConnection sqlite:
                    RebuildSQLiteSchema(connection, addDummyTables);
                    break;

                case MySql.Data.MySqlClient.MySqlConnection mysql:
                    RebuildMySQLSchema(connection, name, addDummyTables);
                    break;

                case System.Data.SqlClient.SqlConnection tsql:
                    RebuildMSSQLSchema(connection, name, addDummyTables);
                    break;
            }
        }

        // ===== TSQL =====

        private static IDbConnection CreateMSSSQLConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
            return new System.Data.SqlClient.SqlConnection(builder.ToString());
        }

        private static void RebuildMSSQLSchema(IDbConnection connection, string schemaName, bool addDummyTables)
        {
            if (string.IsNullOrEmpty(schemaName)) throw new ArgumentNullException(nameof(schemaName));

            if (connection.State != ConnectionState.Open) connection.Open();
            IDbCommand command;
            using (command = connection.CreateCommand())
            {
                command.CommandText = $"DROP DATABASE IF EXISTS [{schemaName}];";
                command.ExecuteNonQuery();
                command.CommandText = $"CREATE DATABASE [{schemaName}];";
                command.ExecuteNonQuery();
            }
            connection.ChangeDatabase(schemaName);
            LoadDataset1(connection);
        }

        private static void LoadDataset1(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = $@"
                CREATE TABLE [dbo].[zombie] (
                    Id INTEGER NOT NULL PRIMARY KEY IDENTITY(1,1),
                    Name VARCHAR(255)
                );

                CREATE TABLE [dbo].[placeholder] (
                    Id INT NOT NULL PRIMARY KEY,
                    Name VARCHAR(255)
                );

                CREATE TABLE [dbo].[service] (
                    Id INTEGER NOT NULL PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL,
                    Subscribers INT NOT NULL,
                    Zombie VARCHAR(255),
                    Zombie_fk INTEGER NOT NULL,
                    CONSTRAINT [{ObjectName.ServiceFK}] FOREIGN KEY (Zombie_fk) REFERENCES placeholder(Id)
                );

                CREATE INDEX [{ObjectName.SubscribersIndex}] ON [service] (Subscribers);
                ";
                command.ExecuteNonQuery();
            }
        }

        // ===== MySQL =====

        private static IDbConnection CreateMySQLConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString) { Database = "sys" };
            return new MySql.Data.MySqlClient.MySqlConnection(builder.ToString());
        }

        private static void RebuildMySQLSchema(IDbConnection connection, string schemaName, bool addDummyTables)
        {
            IDbCommand command;
            if (connection.State != ConnectionState.Open) connection.Open();

            using (command = connection.CreateCommand())
            {
                command.CommandText = $"DROP DATABASE IF EXISTS `{schemaName}`;";
                command.ExecuteNonQuery();
                command.CommandText = $"CREATE DATABASE `{schemaName}`;";
                command.ExecuteNonQuery();
            }

            connection.ChangeDatabase(schemaName);
            using (command = connection.CreateCommand())
            {
                if (addDummyTables)
                {
                    command.CommandText = $@"
                    CREATE TABLE `zombie` (
                        Id INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        Name VARCHAR(255)
                    );

                    CREATE TABLE `placeholder` (
                        Id INT NOT NULL PRIMARY KEY,
                        Name VARCHAR(255)
                    );

                    CREATE TABLE `service` (
                        Id INTEGER NOT NULL PRIMARY KEY,
                        Name VARCHAR(255) NOT NULL,
                        Subscribers INT NOT NULL,
                        Zombie VARCHAR(255),
                        Zombie_fk INTEGER NOT NULL,
                        CONSTRAINT `{ObjectName.ServiceFK}` FOREIGN KEY (Zombie_fk) REFERENCES placeholder(Id)
                    );

                    CREATE INDEX `{ObjectName.SubscribersIndex}` ON `service` (Subscribers);
                    ";

                    command.ExecuteNonQuery();
                }
            }
        }

        // ===== SQLite =====

        private static IDbConnection CreateSQLiteConnection(string fileName)
        {
            string databasePath = Path.Combine(Path.GetTempPath(), nameof(Daterpillar), $"{fileName}.db3");
            string folder = Path.GetDirectoryName(databasePath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if (!File.Exists(databasePath)) SQLiteConnection.CreateFile(databasePath);
            return new SQLiteConnection($"Data Source={databasePath}");
        }

        private static void RebuildSQLiteSchema(IDbConnection connection, bool addDummyTables)
        {
            var databasePath = new SQLiteConnectionStringBuilder(connection.ConnectionString).DataSource;
            if (File.Exists(databasePath)) File.Delete(databasePath);
            SQLiteConnection.CreateFile(databasePath);
            if (connection.State != ConnectionState.Open) connection.Open();

            if (addDummyTables)
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = $@"
                    CREATE TABLE [zombie] (
                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        Name VARCHAR(255)
                    );

                    CREATE TABLE [placeholder] (
                        Id INT NOT NULL PRIMARY KEY,
                        Name VARCHAR(255)
                    );

                    CREATE TABLE [service] (
                        Id INTEGER NOT NULL PRIMARY KEY,
                        Name VARCHAR(255) NOT NULL,
                        Subscribers INT,
                        Zombie VARCHAR(255),
                        Zombie_fk INTEGER NOT NULL,
                        FOREIGN KEY (Zombie_fk) REFERENCES placeholder(Id)
                    );
                    CREATE INDEX {ObjectName.SubscribersIndex} ON [service] (Subscribers);
                    ";
                    command.ExecuteNonQuery();
                }
        }

        private struct ObjectName
        {
            public const string
                ServiceFK = "service_Zombie_fk_TO_placeholder_Id__fk",
                SubscribersIndex = "service__Subscribers_index"
                ;
        }
    }
}