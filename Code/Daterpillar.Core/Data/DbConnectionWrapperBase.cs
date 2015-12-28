using Gigobyte.Daterpillar.Data.Linq;
using System;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Data
{
    public abstract class DbConnectionWrapperBase : IDbConnectionWrapper
    {
        public DbConnectionWrapperBase() : this(QueryStyle.SQL)
        {
        }

        public DbConnectionWrapperBase(QueryStyle style)
        {
            _style = style;
        }

        public ExceptionHandlerDelegate ExceptionHandler;

        public event EventHandler<DbExceptionEventArgs> Error;

        public delegate void ExceptionHandlerDelegate(Exception ex, string command, out bool handled);

        public IEnumerable<TEntity> Execute<TEntity>(Query query)
        {
            return Execute<TEntity>(query.GetValue(minify: true));
        }

        public IEnumerable<TEntity> Execute<TEntity>(string query)
        {
            return FetchData<TEntity>(query);
        }

        public void ExecuteNonQuery(string command)
        {
            CommandQueue.Enqueue(command);
        }

        public void Insert(EntityBase entity)
        {
            CommandQueue.Enqueue(SqlWriter.ConvertToInsertCommand(entity, _style));
        }

        public void Insert(IEnumerable<EntityBase> entities)
        {
            foreach (var entity in entities)
            {
                CommandQueue.Enqueue(SqlWriter.ConvertToInsertCommand(entity, _style));
            }
        }

        public void Delete(EntityBase entity)
        {
            CommandQueue.Enqueue(SqlWriter.ConvertToDeleteCommand(entity, _style));
        }

        public void Delete(IEnumerable<EntityBase> entities)
        {
            foreach (var entity in entities)
            {
                CommandQueue.Enqueue(SqlWriter.ConvertToDeleteCommand(entity, _style));
            }
        }

        public abstract void Commit();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Protected Member

        protected Queue<string> CommandQueue = new Queue<string>();

        protected void RaiseError(DbExceptionEventArgs args)
        {
            Error?.Invoke(this, args);
        }

        protected abstract IEnumerable<TEntity> FetchData<TEntity>(string query);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CommandQueue.Clear();
            }
        }

        #endregion Protected Member

        #region Private Member

        private QueryStyle _style;

        #endregion Private Member
    }
}