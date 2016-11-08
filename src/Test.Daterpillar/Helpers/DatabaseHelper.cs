using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using System;
using System.Data;
using System.Data.Common;
using System.IO;

namespace Tests.Daterpillar.Helpers
{
    public static class DatabaseHelper
    {
        public static IDbConnection CreateSQLiteConnection(string filePath = "")
        {
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "daterpillar-test.db3");
                if (File.Exists(filePath)) File.Delete(filePath);
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

        public static bool TryRunScript(IDbConnection connection, string script, out string error)
        {
            error = "";
            IDbCommand command = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(script))
                {
                    if (connection.State != ConnectionState.Open) connection.Open();
                    command = connection.CreateCommand();

                    string[] statements = script.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var cmd in statements)
                        if (!string.IsNullOrWhiteSpace(cmd))
                        {
                            command.CommandText = cmd.Trim();
                            command.ExecuteNonQuery();
                        }

                    error = "** THE SCRIPT WORKS **";
                }
                else error = "** NOTHING WAS GENERATED **";

                return true;
            }
            catch (DbException ex)
            {
                error = ex.Message;
                System.Diagnostics.Debug.WriteLine(error);
                return false;
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
        }

        public static bool TryDropDatabase(this IDbConnection connection, string dbName, bool dispose = false)
        {
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DROP DATABASE {dbName};";
                    command.ExecuteNonQuery();
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

        public static void CreateSchema(this IDbConnection connection, IScriptBuilder builder, Schema schema, bool dispose = false)
        {
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                builder.Create(schema);

                using (var command = connection.CreateCommand())
                {
                    string script = builder.GetContent();
                    string[] statements = script.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var cmd in statements)
                        if (!string.IsNullOrWhiteSpace(cmd))
                        {
                            command.CommandText = cmd.Trim();
                            command.ExecuteNonQuery();
                        }
                }
            }
            finally { if (dispose) connection.Dispose(); }
        }
    }
}