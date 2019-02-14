using Acklann.Daterpillar.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar
{
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    internal sealed class Database : IDisposable
    {
        public Database(Syntax syntax, string name = null, string connectionString = null)
        {
            _syntax = syntax;
            _databaseName = name?? nameof(Daterpillar);
            var config = JObject.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connections.json")));
            string connStr = connectionString ?? config.SelectToken((syntax).ToString().ToLowerInvariant())?.Value<string>();
            System.Data.Common.DbConnectionStringBuilder builder;
            switch (syntax)
            {
                case Syntax.TSQL:
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        connStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                    builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connStr) { InitialCatalog = "master", ConnectTimeout = 5 };
                    _connection = new System.Data.SqlClient.SqlConnection(builder.ToString());
                    break;

                case Syntax.MySQL:
                    builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connStr) { Database = "sys", ConnectionTimeout = 5 };
                    _connection = new MySql.Data.MySqlClient.MySqlConnection(builder.ToString());
                    break;

                case Syntax.SQLite:
                    _dbFile = Path.Combine(Path.GetTempPath(), $"{_databaseName}.sqlite.db");
                    builder = new System.Data.SQLite.SQLiteConnectionStringBuilder() { DataSource = _dbFile };
                    _connection = new System.Data.SQLite.SQLiteConnection(builder.ToString());
                    break;
            }
        }

        public bool TryExecute(string sql, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                Open();
                foreach (string batch in new Regex(@"(?i)\r?\nGO\r?\n").Split(sql))
                    if (string.IsNullOrEmpty(batch) == false)
                        using (IDbCommand command = _connection.CreateCommand())
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

        public void Refresh()
        {
            switch (_syntax)
            {
                default:
                    throw new ArgumentException($"No database reset method for {_syntax} was found; you need to create one.");

                case Syntax.TSQL:
                    TSqlRefresh();
                    break;

                case Syntax.MySQL:
                    MySqlRefresh();
                    break;

                case Syntax.SQLite:
                    SqliteRefresh();
                    break;
            }
        }

        // ==================== INTERNAL MEMBERS ==================== //

        internal void TSqlRefresh()
        {
            Open();
            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = $"DROP DATABASE IF EXISTS [{_databaseName}];";
                command.ExecuteNonQuery();
                command.CommandText = $"CREATE DATABASE [{_databaseName}];";
                command.ExecuteNonQuery();
            }

            _connection.ChangeDatabase(_databaseName);
            using (IDbCommand command = _connection.CreateCommand())
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

        internal void MySqlRefresh()
        {
            Open();
            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = $"DROP DATABASE IF EXISTS `{_databaseName}`;";
                command.ExecuteNonQuery();
                command.CommandText = $"CREATE DATABASE `{_databaseName}`;";
                command.ExecuteNonQuery();
            }

            _connection.ChangeDatabase(_databaseName);
            using (IDbCommand command = _connection.CreateCommand())
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

        internal void SqliteRefresh()
        {
            TestData.CreateDirectory(_dbFile);
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
            System.Data.SQLite.SQLiteConnection.CreateFile(_dbFile);

            Open();
            using (IDbCommand command = _connection.CreateCommand())
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

        #region IDisposable

        public void Dispose()
        {
            _connection?.Dispose();
        }

        #endregion IDisposable

        #region Private Members

        private readonly Syntax _syntax;
        private readonly string _databaseName;
        private readonly IDbConnection _connection;

        private string _dbFile;

        private void Open()
        {
            if (_connection.State != ConnectionState.Open) _connection.Open();
        }

        private string ToDebuggerDisplay() => _connection?.ConnectionString;

        #endregion Private Members

        public struct ObjectName
        {
            public const string
                ServiceFK = "service_Zombie_fk_TO_placeholder_Id__fk",
                SubscribersIndex = "service__Subscribers_index"
                ;
        }

        public class Sample
        {
            public static Schema CreateInstance()
            {
                var schema = new Schema();
                schema.Add(
                    new Table("zombie"),

                    new Table("placeholder",
                        new Column("Id", SchemaType.INT, autoIncrement: true),
                        new Column("Name", SchemaType.VARCHAR, nullable: true)
                    ),

                    new Table("service",
                        new Column("Id", SchemaType.INT, autoIncrement: true),
                        new Column("Name", SchemaType.VARCHAR),
                        new Column("Subscribers", SchemaType.INT),
                        new Column("Zombie", SchemaType.VARCHAR),
                        new Column("Zombie_fk", SchemaType.INT),

                        new ForeignKey("Zombie_fk", "placeholder", "Id"),
                        new Index(IndexType.Index, new ColumnName("Subscribers"))
                    )
                    );

                return schema;
            }
        }
    }
}