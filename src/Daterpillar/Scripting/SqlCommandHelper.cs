using Acklann.Daterpillar.Modeling;
using System;
using System.Collections.Generic;
using System.Data;

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

            IDbCommand command = null;
            IDataReader records = null;

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

        public static SqlCommandResult Insert(this IDbConnection connection, Modeling.IInsertable model, Language dialect)
        {
            return ExecuteCommand(connection, SqlComposer.ToInsertCommand(model, dialect), dialect);
        }

        public static QueryResult<TRecord> SelectOne<TRecord>(this IDbConnection connection, string query, Language dialect) where TRecord : Modeling.ISelectable
        {
            throw new System.NotImplementedException();
        }

        public static SqlCommandResult ExecuteCommand(this IDbConnection connection, string sql, Language dialect)
        {
            IDbCommand command = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                try { connection.ChangeDatabase(nameof(Daterpillar)); } catch (System.Data.Common.DbException) { }

                command = connection.CreateCommand();
                command.CommandText = sql;
                long changes = command.ExecuteNonQuery();
                return new SqlCommandResult(changes, 0, null);
            }
            catch (System.Data.Common.DbException ex)
            {
                return new SqlCommandResult(GetSqlErrorCode(ex, dialect), GetSqlErrorCode(ex, dialect), ex.Message);
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
                case Language.MySQL:
                    return Convert.ToInt32(exception.GetType().GetProperty("Number")?.GetValue(exception));

                default: return exception.ErrorCode;
            }
        }
    }
}