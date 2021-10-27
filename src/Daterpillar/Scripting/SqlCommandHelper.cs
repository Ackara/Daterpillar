using Acklann.Daterpillar.Modeling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Acklann.Daterpillar.Scripting
{
    public static class SqlCommandHelper
    {
        public static QueryResult Select(this IDbConnection connection, string query, Type recordType)
        {
            if (string.IsNullOrEmpty(query)) return new QueryResult(new object[0]);

            IDbCommand command = null;
            IDataReader records = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                command = connection.CreateCommand();
                command.CommandText = query;
                records = command.ExecuteReader();

                ISelectable record;
                var results = new LinkedList<ISelectable>();

                while (records.Read())
                {
                    record = (ISelectable)Activator.CreateInstance(recordType);
                    record.Load(records);
                    results.AddLast(record);
                }

                return new QueryResult(results);
            }
            catch (System.Data.Common.DbException ex)
            {
                return new QueryResult(new object[0], ex.Message);
            }
            finally
            {
                command?.Dispose();
                records?.Dispose();
            }
        }

        public static QueryResult<IEnumerable<TModel>> Select<TModel>(this IDbConnection connection, string query) where TModel : ISelectable
        {
            if (string.IsNullOrEmpty(query)) return new QueryResult<IEnumerable<TModel>>(new TModel[0]);
            IDataReader records = null;
            IDbCommand command = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                command = connection.CreateCommand();
                command.CommandText = query;
                records = command.ExecuteReader();

                TModel record;
                var results = new LinkedList<TModel>();

                while (records.Read())
                {
                    record = Activator.CreateInstance<TModel>();
                    record.Load(records);
                    results.AddLast(record);
                }

                return new QueryResult<IEnumerable<TModel>>(results);
            }
            catch (System.Data.Common.DbException ex)
            {
                return new QueryResult<IEnumerable<TModel>>(new TModel[0], ex.Message);
            }
            finally
            {
                command?.Dispose();
                records?.Dispose();
            }
        }

        public static QueryResult<TRecord> SelectOne<TRecord>(this IDbConnection connection, string query) where TRecord : ISelectable
        {
            if (string.IsNullOrEmpty(query)) return default;
            IDataReader results = null;
            IDbCommand command = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                command = connection.CreateCommand();
                command.CommandText = query;
                results = command.ExecuteReader();

                while (results.Read())
                {
                    TRecord record = Activator.CreateInstance<TRecord>();
                    record.Load(results);
                    return new QueryResult<TRecord>(record);
                }
            }
            catch (System.Data.Common.DbException ex)
            {
                return new QueryResult<TRecord>(default, ex.Message);
            }
            finally
            {
                command?.Dispose();
                results?.Dispose();
            }

            return new QueryResult<TRecord>(default, "no records");
        }

        public static SqlCommandResult Insert(this IDbConnection connection, Language connectionType, params IInsertable[] models)
        {
            return ExecuteCommand(connection, connectionType, models.Select(x => SqlComposer.ToInsertCommand(x, connectionType)).ToArray());
        }

        public static SqlCommandResult ExecuteCommand(this IDbConnection connection, string sql, Language dialect)
        {
            return ExecuteCommand(connection, dialect, sql);
        }

        public static SqlCommandResult ExecuteCommand(this IDbConnection connection, Language connectionType, params string[] commands)
        {
            IDbTransaction transaction = null;
            IDbCommand command = null;
            long changes = 0;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                transaction = connection.BeginTransaction();
                command = connection.CreateCommand();
                command.Transaction = transaction;

                for (int i = 0; i < commands.Length; i++)
                {
                    command.CommandText = commands[i];
                    if (!string.IsNullOrEmpty(command.CommandText)) changes += command.ExecuteNonQuery();
                }

                transaction.Commit();
                return new SqlCommandResult(true, 0, changes, null);
            }
            catch (System.Data.Common.DbException ex)
            {
                transaction?.Rollback();
                return new SqlCommandResult(false, GetSqlErrorCode(ex, connectionType), changes, ex.Message);
            }
            finally
            {
                command?.Dispose();
                transaction?.Dispose();
            }
        }

        public static SqlCommandResult[] ExecuteCommands(this IDbConnection connection, Language connectionType, params string[] commands)
        {
            var results = new SqlCommandResult[commands.Length];
            IDbTransaction transaction = null;
            IDbCommand command = null;
            long changes = 0;
            int index = 0;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                transaction = connection.BeginTransaction();
                command = connection.CreateCommand();
                command.Transaction = transaction;

                for (index = 0; index < commands.Length; index++)
                {
                    command.CommandText = commands[index];
                    if (!string.IsNullOrEmpty(command.CommandText)) changes = command.ExecuteNonQuery();
                    results[index] = new SqlCommandResult(true, 0, changes, null);
                }
            }
            catch (System.Data.Common.DbException ex)
            {
                transaction?.Rollback();
                results[index] = new SqlCommandResult(false, GetSqlErrorCode(ex, connectionType), changes, ex.Message);
            }
            finally
            {
                transaction?.Dispose();
                command?.Dispose();
            }

            return results;
        }

        public static Language GetLanguage(this IDbConnection connection) => GetLanguage(connection.GetType());

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

        internal static int GetSqlErrorCode(System.Data.Common.DbException exception, Language connectionType)
        {
            int code;
            switch (connectionType)
            {
                case Language.TSQL:
                case Language.MySQL:
                    code = Convert.ToInt32(exception.GetType().GetProperty("Number")?.GetValue(exception));
                    break;

                default: return exception.ErrorCode;
            }

            return code == 0 ? 500 : code;
        }
    }
}