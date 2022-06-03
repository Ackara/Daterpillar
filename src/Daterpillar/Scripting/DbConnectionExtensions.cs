using System;
using System.Data;
using System.Data.Common;

namespace Acklann.Daterpillar.Scripting
{
    public static class DbConnectionExtensions
    {
        public static SqlCommandResult Insert(this IDbConnection connection, Language connectionType, object record)
        {
            IDbCommand command = null;

            try
            {
                OpenConnection(connection);
                command = connection.CreateCommand();
                command.Transaction = connection.BeginTransaction();
                CrudOperations.Create2(command, record, connectionType);
                int changes = command.ExecuteNonQuery();
                command.Transaction.Commit();
                return new SqlCommandResult(true, 0, changes, null);
            }
            catch (DbException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                command.Transaction.Rollback();
                return new SqlCommandResult(false, 0, 0, ex.Message);
            }
            finally
            {
                command?.Dispose();
            }
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

        private static void OpenConnection(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
        }
    }
}