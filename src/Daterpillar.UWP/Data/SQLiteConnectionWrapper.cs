using SQLitePCL;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Data
{
    /// <summary>
    /// Represents a SQLite connection.
    /// </summary>
    /// <seealso cref="Gigobyte.Daterpillar.Data.DbConnectionWrapperBase" />
    public class SQLiteConnectionWrapper : DbConnectionWrapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConnectionWrapper"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SQLiteConnectionWrapper(string connectionString) : this(connectionString, true, new SQLitePclEntityConstruction())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConnectionWrapper"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="enableForeignKeys">if set to <c>true</c> [enable foreign keys].</param>
        public SQLiteConnectionWrapper(string connectionString, bool enableForeignKeys) : this(connectionString, enableForeignKeys, new SQLitePclEntityConstruction())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConnectionWrapper"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="enableForeignKeys">if set to <c>true</c> [enable foreign keys].</param>
        /// <param name="constructor">The constructor.</param>
        public SQLiteConnectionWrapper(string connectionString, bool enableForeignKeys, IEntityConstructor constructor) : base(Linq.QueryStyle.SQLite)
        {
            Constructor = constructor;
            ConnectionString = connectionString;
            CommandQueue.Enqueue($"PRAGMA foreign_keys = {(enableForeignKeys ? 1 : 0)};");
        }

        /// <summary>
        /// Commits or save changes made on this open connection.
        /// </summary>
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

        /// <summary>
        /// The connection string.
        /// </summary>
        protected readonly string ConnectionString;

        /// <summary>
        /// The constructor.
        /// </summary>
        protected IEntityConstructor Constructor;

        /// <summary>
        /// The SQLite connection.
        /// </summary>
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

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <param name="mode">The mode.</param>
        protected void OpenConnection()
        {
            if (Connection == null)
            {
                Connection = new SQLiteConnection(ConnectionString, SQLiteOpen.READWRITE);
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