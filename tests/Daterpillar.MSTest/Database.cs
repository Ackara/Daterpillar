using Acklann.Daterpillar.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;

namespace Acklann.Daterpillar
{
    internal sealed class Database : IDisposable
    {
        public Database(Syntax syntax)
        {
            _syntax = syntax;
            var config = JObject.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connections.json")));
            string connectionStr = config.SelectToken($"{syntax}.connectionString")?.Value<string>();

            switch (syntax)
            {
                case Syntax.MSSQL:
                    _connection = new System.Data.SqlClient.SqlConnection(connectionStr);
                    break;

                case Syntax.MySQL:
                    _connection = new MySql.Data.MySqlClient.MySqlConnection(connectionStr);
                    break;

                case Syntax.SQLite:
                    _connection = new System.Data.SQLite.SQLiteConnection(new System.Data.SQLite.SQLiteConnectionStringBuilder() { DataSource = _sqliteFile }.ConnectionString);
                    break;
            }
        }

        public bool TryExecute(string sql, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                Open();

                using (IDbCommand command = _connection.CreateCommand())
                {
                    command.CommandText = sql;
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

        #region Database Resets

        public static void Refresh()
        {
            MssqlRefresh();
            SqliteRefresh();
        }

        public static void Refresh(Syntax syntax)
        {
            switch (syntax)
            {
                case Syntax.MSSQL:
                    MssqlRefresh();
                    break;

                case Syntax.SQLite:
                    SqliteRefresh();
                    break;

                default:
                    throw new ArgumentException($"No database reset method for {syntax} was found; you need to create one.");
            }
        }

        public static void SqliteRefresh()
        {
            TestData.CreateDirectory(_sqliteFile);
            if (File.Exists(_sqliteFile)) File.Delete(_sqliteFile);
            System.Data.SQLite.SQLiteConnection.CreateFile(_sqliteFile);

            using (var db = new Database(Syntax.SQLite))
            {
                db.Open();
                using (IDbCommand command = db._connection.CreateCommand())
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
CREATE INDEX service_Subscribers_index ON [service] (Subscribers);
";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void MssqlRefresh()
        {
            using (var db = new Database(Syntax.MSSQL))
            {
                db.Open();
                using (IDbCommand command = db._connection.CreateCommand())
                {
                    command.CommandText = @"
DROP TABLE IF EXISTS [dbo].[service];
DROP TABLE IF EXISTS [dbo].[zombie];
DROP TABLE IF EXISTS [dbo].[placeholder];

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
    Subscribers INT,
    Zombie VARCHAR(255),
    Zombie_fk INTEGER NOT NULL,
    FOREIGN KEY (Zombie_fk) REFERENCES placeholder(Id)
);

CREATE INDEX service_Subscribers_index ON [service] (Subscribers);
";
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion Database Resets

        #region IDisposable

        public void Dispose()
        {
            _connection?.Dispose();
        }

        #endregion IDisposable

        #region Private Members

        private static string _sqliteFile = Path.Combine(Path.GetTempPath(), "dtp-sqlite.db");

        private readonly Syntax _syntax;
        private readonly IDbConnection _connection;

        private void Open()
        {
            if (_connection.State != ConnectionState.Open) _connection.Open();
        }

        #endregion Private Members

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