using Acklann.Daterpillar.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;

namespace Acklann.Daterpillar
{
    internal sealed class Database : IDisposable
    {
        public Database(Syntax syntax, string name = nameof(Daterpillar))
        {
            _syntax = syntax;
            var config = JObject.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connections.json")));
            string connectionStr = config.SelectToken($"{syntax}.connectionString")?.Value<string>();
            System.Data.Common.DbConnectionStringBuilder builder;
            switch (syntax)
            {
                case Syntax.MSSQL:
                    builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionStr) { InitialCatalog = "master" };
                    _connection = new System.Data.SqlClient.SqlConnection(builder.ToString());
                    break;

                case Syntax.MySQL:
                    builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionStr) { Database = name };
                    _connection = new MySql.Data.MySqlClient.MySqlConnection(builder.ToString());
                    break;

                case Syntax.SQLite:
                    _dbFile = Path.Combine(Path.GetTempPath(), $"{name}.sqlite.db");
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

        public void Refresh()
        {
            switch (_syntax)
            {
                default:
                    throw new ArgumentException($"No database reset method for {_syntax} was found; you need to create one.");

                case Syntax.MSSQL:
                    MssqlRefresh();
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

        internal void MssqlRefresh()
        {
            Open();
            using (IDbCommand command = _connection.CreateCommand())
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
CREATE INDEX service_Subscribers_index ON [service] (Subscribers);
";
                command.ExecuteNonQuery();
            }
        }

        internal void MySqlRefresh()
        {
            throw new System.NotImplementedException();
        }

        #region IDisposable

        public void Dispose()
        {
            _connection?.Dispose();
        }

        #endregion IDisposable

        #region Private Members

        private readonly Syntax _syntax;
        private readonly IDbConnection _connection;

        private string _dbFile;

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