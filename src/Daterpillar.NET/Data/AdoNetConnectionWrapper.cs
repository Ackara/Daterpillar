using Gigobyte.Daterpillar.Data.Linq;
using System.Collections.Generic;
using System.Data;

namespace Gigobyte.Daterpillar.Data
{
    public class AdoNetConnectionWrapper : DbConnectionWrapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetConnectionWrapper"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public AdoNetConnectionWrapper(IDbConnection connection) : this(connection, new AdoNetEntityConstructor(), QueryStyle.SQL)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetConnectionWrapper"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="style">The style.</param>
        public AdoNetConnectionWrapper(IDbConnection connection, QueryStyle style) : this(connection, new AdoNetEntityConstructor(), style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetConnectionWrapper"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="constructor">The constructor.</param>
        /// <param name="style">The style.</param>
        public AdoNetConnectionWrapper(IDbConnection connection, IEntityConstructor constructor, QueryStyle style) : base(style)
        {
            _constructor = constructor;
            Connection = connection;
        }

        /// <summary>
        /// The connection.
        /// </summary>
        protected IDbConnection Connection;

        /// <summary>
        /// Commits or save changes made on this open connection.
        /// </summary>
        public override void Commit()
        {
            OpenConnectionIfClosed();

            IDbTransaction transaction = Connection.BeginTransaction();
            IDbCommand command = Connection.CreateCommand();
            command.Transaction = transaction;

            try
            {
                while (CommandQueue.Count > 0)
                {
                    command.CommandText = CommandQueue.Dequeue();
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (System.Data.Common.DbException ex)
            {
                bool handled = false;
                ExceptionHandler?.Invoke(ex, command.CommandText, out handled);

                if (handled == false)
                {
                    RaiseError(new DbExceptionEventArgs(command.CommandText, ex.Message, ex.ErrorCode));
                    transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                command.Dispose();
                transaction.Dispose();
            }
        }

        protected override IEnumerable<TEntity> FetchData<TEntity>(string query)
        {
            OpenConnectionIfClosed();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = query;
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {
                        yield return (TEntity)_constructor.CreateInstance(typeof(TEntity), row);
                    }
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Connection?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Private Member

        private IEntityConstructor _constructor;

        private void OpenConnectionIfClosed()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        #endregion Private Member
    }
}