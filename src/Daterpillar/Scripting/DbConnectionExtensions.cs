using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Acklann.Daterpillar.Scripting
{
    public static class DbConnectionExtensions
    {
        public static SqlCommandResult Insert(this IDbConnection connection, object record, Language connectionType = Language.SQL)
        {
            IDbCommand command = connection.CreateCommand();

            try
            {
                OpenConnection(connection);
                CrudOperations.Create(command, record, connectionType);
                command.Transaction = connection.BeginTransaction();
                int changes = command.ExecuteNonQuery();
                command.Transaction.Commit();
                return new SqlCommandResult(command.CommandText, changes);
            }
            catch (DbException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                command.Transaction.Rollback();
                return new SqlCommandResult(command.CommandText, GetSqlErrorCode(ex, connectionType), ex.Message);
            }
            finally
            {
                command?.Dispose();
            }
        }

        public static SqlCommandResult Update(this IDbConnection connection, object record, Language connectionType = Language.SQL)
        {
            IDbCommand command = connection.CreateCommand();

            try
            {
                OpenConnection(connection);
                command.Transaction = connection.BeginTransaction();
                CrudOperations.Update(command, record, connectionType);
                int changes = command.ExecuteNonQuery();
                command.Transaction.Commit();
                return new SqlCommandResult(command.CommandText, changes);
            }
            catch (DbException ex)
            {
                command.Transaction.Rollback();
                return new SqlCommandResult(command.CommandText, GetSqlErrorCode(ex, connectionType), ex.Message);
            }
            finally
            {
                command.Dispose();
            }
        }

        public static SqlCommandResult Delete(this IDbConnection connection, object record, Language connectionType = Language.SQL)
        {
            IDbCommand command = connection.CreateCommand();

            try
            {
                OpenConnection(connection);
                command.Transaction = connection.BeginTransaction();
                CrudOperations.Delete(command, record, connectionType);
                int changes = command.ExecuteNonQuery();
                command.Transaction.Commit();
                return new SqlCommandResult(command.CommandText, changes);
            }
            catch (DbException ex)
            {
                command.Transaction.Rollback();
                return new SqlCommandResult(command.CommandText, GetSqlErrorCode(ex, connectionType), ex.Message);
            }
            finally
            {
                command.Dispose();
            }
        }

        public static QueryResult<IEnumerable<TRecord>> Select<TRecord>(this IDbConnection connection, string query)
        {
            IDbCommand command = connection.CreateCommand();

            try
            {
                OpenConnection(connection);
                command.CommandText = query;
                using (IDataReader reader = command.ExecuteReader())
                {
                    return new QueryResult<IEnumerable<TRecord>>(CrudOperations.Read<TRecord>(reader), null);
                }
            }
            catch (DbException ex)
            {
                return new QueryResult<IEnumerable<TRecord>>(Array.Empty<TRecord>(), ex.Message);
            }
            finally
            {
                command.Dispose();
            }
        }

        public static QueryResult<TRecord> SelectOne<TRecord>(this IDbConnection connection, string query)
        {
            IDbCommand command = connection.CreateCommand();

            try
            {
                OpenConnection(connection);
                command.CommandText = query;
                using (IDataReader reader = command.ExecuteReader())
                {
                    return new QueryResult<TRecord>(CrudOperations.Read<TRecord>(reader).FirstOrDefault(), null);
                }
            }
            catch (DbException ex)
            {
                return new QueryResult<TRecord>(default, ex.Message);
            }
            finally
            {
                command.Dispose();
            }
        }

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

        internal static int GetSqlErrorCode(DbException exception, Language connectionType)
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

        private static void OpenConnection(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
        }
    }
}