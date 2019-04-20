using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Writers;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Migration
{
    public static class IDbConnectionExtensions
    {
        // TODO: Create a method that would query the database for its tables then remove all of them.

        public static Language GetLanguage(Type connectionType)
        {
            string name = connectionType.Name;
            if (name.Contains("SQLite"))
                return Language.SQLite;
            else if (name.Contains("MySql"))
                return Language.MySQL;
            else if (name.Contains("Sql"))
                return Language.TSQL;

            throw new InvalidCastException($"Could not cast '{connectionType.Name}' to '{nameof(Language)}'.");
        }

        public static Language GetLanguage(this IDbConnection connection) => GetLanguage(connection.GetType());

        public static IDbConnection Open(this IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (connection.State != ConnectionState.Open) connection.Open();
            return connection;
        }

        // ==================== DROP DATABASE ==================== //

        public static IDbConnection RemoveDatabase(this IDbConnection connection, string name, Language kind)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (kind == Language.SQLite)
            {
                return RemoveSQLiteDatabase(connection);
            }

            Open(connection);

            var factory = new SqlWriterFactory();
            using (var data = new MemoryStream())
            using (SqlWriter writer = factory.CreateInstance(kind, data))
            using (IDbCommand command = connection.CreateCommand())
            {
                writer.Drop(name);
                writer.Flush();
                command.CommandText = Encoding.UTF8.GetString(data.ToArray());
                command.ExecuteNonQuery();
            }

            return connection;
        }

        // ==================== CREATE DATABASE ==================== //

        public static IDbConnection CreateDatabase(this IDbConnection connection, Schema schema, Language kind, bool dropIfExists = false)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (schema == null) throw new ArgumentNullException(nameof(schema));
            string schemaName = schema.ResolveName();
            if (string.IsNullOrEmpty(schemaName)) throw new ArgumentNullException(nameof(schema), $"You did not give the schema a name.");

            if (kind == Language.SQLite)
            {
                return CreateSQLiteDatabase(connection, schema, dropIfExists);
            }

            Open(connection);

            // I am checking if the database already exist by trying to change to it.
            bool schemaNotFound = false;
            string original = connection.Database;// I am saving this now for when I need to drop the database.
            try { connection.ChangeDatabase(schemaName); }
            catch (NotImplementedException) { schemaNotFound = true; }
            catch (System.Data.Common.DbException) { schemaNotFound = true; }

            if (schemaNotFound || dropIfExists)
            {
                IDbCommand command;
                var factory = new SqlWriterFactory();

                if (dropIfExists)
                    using (var data = new MemoryStream())
                    using (SqlWriter writer = factory.CreateInstance(kind, new StreamWriter(data)))
                    {
                        connection.ChangeDatabase(original);
                        RemoveDatabase(connection, schemaName, kind);
                        using (command = connection.CreateCommand())
                        {
                            writer.Create(schemaName);
                            writer.Flush();
                            command.CommandText = Encoding.UTF8.GetString(data.ToArray());
                            command.ExecuteNonQuery();
                        }
                    }

                using (var data = new MemoryStream())
                using (SqlWriter writer = factory.CreateInstance(kind, data))
                {
                    connection.ChangeDatabase(schemaName);
                    using (command = connection.CreateCommand())
                    {
                        schema.Merge();
                        writer.Create(schema);
                        writer.Flush();
                        command.CommandText = Encoding.UTF8.GetString(data.ToArray());
                        command.ExecuteNonQuery();
                    }
                }
            }

            return connection;
        }

        public static IDbConnection CreateDatabase(this IDbConnection connection, Assembly assembly, Language kind, string schemaName, bool dropIfExists = false)
        {
            var schema = SchemaFactory.CreateFrom(assembly);
            schema.Name = schemaName;
            return CreateDatabase(connection, schema, kind, dropIfExists);
        }

        public static IDbConnection CreateDatabase(this IDbConnection connection, string assemblyPath, string name, Language kind, bool dropIfExists = false)
        {
            var schema = SchemaFactory.CreateFrom(assemblyPath);
            schema.Name = name;
            return CreateDatabase(connection, schema, kind, dropIfExists);
        }

        private static IDbConnection CreateSQLiteDatabase(IDbConnection connection, Schema schema, bool dropIfExists)
        {
            if (dropIfExists) RemoveSQLiteDatabase(connection);

            var factory = new SqlWriterFactory();
            using (var data = new MemoryStream())
            using (var writer = factory.CreateInstance(Language.SQLite, new StreamWriter(data)))
            {
                schema.Merge();
                writer.Create(schema);
                writer.Flush();

                Open(connection);
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = Encoding.UTF8.GetString(data.ToArray());
                    command.ExecuteNonQuery();
                }
            }

            return connection;
        }

        private static IDbConnection RemoveSQLiteDatabase(IDbConnection connection)
        {
            string databasePath = GetSQLiteDatabasePath(connection.ConnectionString);
            if (string.IsNullOrEmpty(databasePath)) throw new FileNotFoundException($"Could not find SQLite database path from the connectionString '{connection.ConnectionString}'. Make sure the connection string has 'Data Souce' key.");

            if (File.Exists(databasePath)) File.Delete(databasePath);
            return connection;
        }

        private static string GetSQLiteDatabasePath(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var pattern = new Regex("(?i)data.source=(?<path>[^;]+);?");
            Match match = pattern.Match(connectionString);
            if (match.Success)
            {
                return match.Groups["path"].Value;
            }

            return null;
        }
    }
}