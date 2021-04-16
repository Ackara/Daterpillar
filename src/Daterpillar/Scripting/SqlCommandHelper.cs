using System;
using System.Data;

namespace Acklann.Daterpillar.Scripting
{
    public static class SqlCommandHelper
    {
        public static QueryResult<TModel[]> Select<TModel>(this IDbConnection connection, string query) where TModel : Modeling.ISelectable
        {
            if (string.IsNullOrEmpty(query)) return new QueryResult<TModel[]>(new TModel[0]);
            throw new System.NotImplementedException();
        }

        public static SqlCommandResult Insert(this IDbConnection connection, Modeling.IInsertable model, Language dialect)
        {
            IDbCommand command = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                command = connection.CreateCommand();

                command.CommandText = SqlComposer2.ToInsertCommand(model, dialect);
                long changes = command.ExecuteNonQuery();
                return new SqlCommandResult(changes, 0, null);
            }
            catch (System.Data.Common.DbException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new SqlCommandResult(0, GetSqlErrorCode(ex, dialect), ex.Message);
            }
            finally
            {
                command?.Dispose();
            }
        }

        public static QueryResult<TRecord> SelectOne<TRecord>(this IDbConnection connection, string query, Language dialect) where TRecord : Modeling.ISelectable
        {
            try
            {
            }
            catch (System.Data.Common.DbException ex)
            {
                return new QueryResult<TRecord>(default, ex.Message);
            }
            throw new System.NotImplementedException();
        }

        public static SqlCommandResult ExecuteCommand(this IDbConnection connection, string sql, Language dialect)
        {
            IDbCommand command = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                command = connection.CreateCommand();
                command.CommandText = sql;
                long changes = command.ExecuteNonQuery();
                return new SqlCommandResult(changes, 0, null);
            }
            catch (System.Data.Common.DbException ex)
            {
                return new SqlCommandResult(0, GetSqlErrorCode(ex, dialect), ex.Message);
            }
            finally
            {
                command?.Dispose();
            }
        }

        internal static int GetSqlErrorCode(System.Data.Common.DbException exception, Language connectionType)
        {
            switch (connectionType)
            {
                case Language.TSQL:
                    return Convert.ToInt32(exception.GetType().GetProperty("Number")?.GetValue(exception));

                case Language.MySQL:
                    return Convert.ToInt32(exception.GetType().GetProperty("Code")?.GetValue(exception));

                default: return exception.ErrorCode;
            }
        }
    }
}