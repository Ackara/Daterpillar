using SQLitePCL;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Data
{
    public class SQLiteConnectionWrapper : DbConnectionWrapperBase
    {
        public SQLiteConnectionWrapper(string connectionString) : this(connectionString, new SQLitePclEntityConstruction())
        {
        }

        public SQLiteConnectionWrapper(string connectionString, IEntityConstructor constructor) : base(Linq.QueryStyle.SQLite)
        {
            ConnectionString = connectionString;
        }

        public override void Commit()
        {
            OpenConnection();

            // TODO: Begin Transaction here if possible

            string command;
            while (CommandQueue.Count > 0)
            {
                command = CommandQueue.Dequeue();
                try
                {
                    using (ISQLiteStatement statement = Connection.Prepare(command))
                    {
                        statement.Step();
                        statement.Reset();
                        statement.ClearBindings();
                    }

                    // TODO: Commit here if possible
                }
                catch (SQLiteException ex)
                {
                    bool handled = false;
                    ExceptionHandler?.Invoke(ex, command, out handled);

                    if (handled == false)
                    {
                        RaiseError(new DbExceptionEventArgs(command, ex.Message, ex.HResult));
                        // TODO: Roolback here if possible
                        throw;
                    }
                }
            }
        }

        #region Protected Members

        protected readonly string ConnectionString;

        protected IEntityConstructor Constructor;

        protected ISQLiteConnection Connection;

        protected override IEnumerable<TEntity> FetchData<TEntity>(string query)
        {
            OpenConnection();

            using (ISQLiteStatement statement = Connection.Prepare(query))
            {
                while (statement.Step() == SQLiteResult.ROW)
                {
                    yield return (TEntity)Constructor.CreateInstance(typeof(TEntity), statement);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Connection?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected void OpenConnection(SQLiteOpen mode = SQLiteOpen.READWRITE)
        {
            if (Connection == null)
            {
                Connection = new SQLiteConnection(ConnectionString, mode);
            }
        }

        #endregion Protected Members
    }
}