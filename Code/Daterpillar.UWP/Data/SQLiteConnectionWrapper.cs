using SQLitePCL;
using SQLitePCL.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigobyte.Daterpillar.Data
{
    public class SQLiteConnectionWrapper : DbConnectionWrapperBase
    {
        public SQLiteConnectionWrapper(string connectionString, IEntityConstructor constructor)
        {
            _connectionString = connectionString;
        }

        public override void Commit()
        {
            OpenConnection();

            string command;
            while (CommandQueue.Count > 0)
            {
                command = CommandQueue.Dequeue();

            }
        }

        #region Protected Members
        protected override IEnumerable<TEntity> FetchData<TEntity>(string query)
        {
            OpenConnection();

            using (ISQLiteStatement statement = Connection.Prepare(query))
            {
                while (statement.Step() == SQLiteResult.ROW)
                {
                    yield return (TEntity)_constructor.CreateInstance(typeof(TEntity), statement);
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



        protected ISQLiteConnection Connection;
        #endregion

        #region Private Members
        private readonly string _connectionString;
        private IEntityConstructor _constructor;

        private void OpenConnection()
        {
            if (Connection == null)
            {
                Connection = new SQLiteConnection(_connectionString, SQLiteOpen.READWRITE);
            }
        }
        #endregion
    }
}
