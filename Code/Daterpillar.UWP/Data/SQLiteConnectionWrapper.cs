using SQLitePCL;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Data
{
    public class SQLiteConnectionWrapper : DbConnectionWrapperBase
    {
        public SQLiteConnectionWrapper(string connectionString) : this(connectionString, true, new SQLitePclEntityConstruction())
        {
        }

        public SQLiteConnectionWrapper(string connectionString, bool enableForeignKeys) : this(connectionString, enableForeignKeys, new SQLitePclEntityConstruction())
        {
        }

        public SQLiteConnectionWrapper(string connectionString, bool enableForeignKeys, IEntityConstructor constructor) : base(Linq.QueryStyle.SQLite)
        {
            Constructor = constructor;
            ConnectionString = connectionString;
            CommandQueue.Enqueue($"PRAGMA foreign_keys = {(enableForeignKeys ? 1 : 0)};");
        }

        public override void Commit()
        {
            OpenConnection();

            string command;
            ISQLiteStatement statement;

            while (CommandQueue.Count > 0)
            {
                command = CommandQueue.Dequeue();
                try
                {
                    using (statement = Connection.Prepare(command))
                    {
                        statement.Step();
                    }
                }
                catch (SQLiteException ex)
                {
                    bool handled = false;
                    ExceptionHandler?.Invoke(ex, command, out handled);

                    if (handled == false)
                    {
                        RaiseError(new DbExceptionEventArgs(command, ex.Message, ex.HResult));
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

        protected void OpenConnection(SQLiteOpen mode = SQLiteOpen.READWRITE)
        {
            if (Connection == null)
            {
                Connection = new SQLiteConnection(ConnectionString, mode);
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

        #endregion Protected Members
    }
}