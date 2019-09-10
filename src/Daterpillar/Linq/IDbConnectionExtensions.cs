using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Writers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Acklann.Daterpillar.Linq
{
    public static class IDbConnectionExtensions
    {
        // TODO: Create a method that would query the database for its tables then remove all of them.

        public static Language GetLanguage(Type connectionType)
        {
            string name = connectionType.Name.ToLowerInvariant();
            if (name.Contains("sqlite"))
                return Language.SQLite;
            else if (name.Contains("mysql"))
                return Language.MySQL;
            else if (name.Contains("sql"))
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

        // ==================== SELECT DATA ==================== //

        public static IEnumerable<TEntity> Select<TEntity>(this IDbConnection connection, string query) where TEntity : IEntity
        {
            if (string.IsNullOrEmpty(query)) yield break;

            Open(connection);
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (IDataReader result = command.ExecuteReader())
                {
                    TEntity entity;
                    while (result.Read())
                    {
                        entity = Activator.CreateInstance<TEntity>();
                        entity.Load(result);
                        yield return entity;
                    }
                }
            }
        }

        public static TEntity SelectOne<TEntity>(this IDbConnection connection, string query) where TEntity : IEntity
        {
            if (string.IsNullOrEmpty(query)) return default;

            Open(connection);
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (IDataReader result = command.ExecuteReader())
                {
                    if (result.Read())
                    {
                        TEntity entity = Activator.CreateInstance<TEntity>();
                        entity.Load(result);
                        return entity;
                    }
                }
            }

            return default;
        }

        public static Task<TEntity[]> SelectAsync<TEntity>(this IDbConnection connection, string query, System.Threading.CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return Task.Run(() => Select<TEntity>(connection, query).ToArray(), cancellationToken);
        }

        public static Task<TEntity> SelectOneAsync<TEntity>(this IDbConnection connection, string query, System.Threading.CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return Task.Run(() => SelectOne<TEntity>(connection, query), cancellationToken);
        }

        // ==================== INSERT DATA ==================== //

        public static bool Insert(this IDbConnection connection, params IEntity[] entities)
        {
            return Insert(connection, GetLanguage(connection), entities);
        }

        public static bool Insert(this IDbConnection connection, Language kind, params IEntity[] entities)
        {
            Open(connection);
            using (IDbTransaction transaction = connection.BeginTransaction())
                try
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        foreach (string sql in SqlComposer.GenerateInsertStatements(kind, entities))
                        {
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch { transaction.Rollback(); }
            return false;
        }

        public static Task<bool> InsertAsync(this IDbConnection connection, params IEntity[] entities)
        {
            return InsertAsync(connection, GetLanguage(connection), entities);
        }

        public static Task<bool> InsertAsync(this IDbConnection connection, Language lang, params IEntity[] entities)
        {
            return Task.Run(() => { return Insert(connection, lang, entities); });
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

        public static IDbConnection CreateDatabase(this IDbConnection connection, Assembly assembly, string name, Language kind, bool dropIfExists = false)
        {
            var schema = Migration.SchemaFactory.CreateFrom(assembly);
            schema.Name = name;
            return CreateDatabase(connection, schema, kind, dropIfExists);
        }

        public static IDbConnection CreateDatabase(this IDbConnection connection, string assemblyPath, string name, Language kind, bool dropIfExists = false)
        {
            var schema = Migration.SchemaFactory.CreateFrom(assemblyPath);
            schema.Name = name;
            return CreateDatabase(connection, schema, kind, dropIfExists);
        }

        // ==================== SQLITE ==================== //

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