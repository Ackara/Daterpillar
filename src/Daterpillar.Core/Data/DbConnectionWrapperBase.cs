using Acklann.Daterpillar.Linq;
using System;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Data
{
    /// <summary>
    /// </summary>
    /// <seealso cref="IDbConnectionWrapper"/>
    public abstract class DbConnectionWrapperBase : IDbConnectionWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionWrapperBase"/> class.
        /// </summary>
        public DbConnectionWrapperBase() : this(QueryStyle.SQL)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionWrapperBase"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public DbConnectionWrapperBase(QueryStyle style)
        {
            _style = style;
            
        }

        /// <summary>
        /// Intercept non handled database exceptions.
        /// </summary>
        public ExceptionHandlerDelegate ExceptionHandler;

        /// <summary>
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="command">The command.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        public delegate void ExceptionHandlerDelegate(Exception ex, string command, out bool handled);

        /// <summary>
        /// Occurs when connected database throw an error.
        /// </summary>
        public event EventHandler<DbExceptionEventArgs> Error;

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Execute<TEntity>(Query query)
        {
            return Execute<TEntity>(query.GetValue(minify: true));
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Execute<TEntity>(string query)
        {
            return FetchData<TEntity>(query);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="command">The command.</param>
        public void ExecuteNonQuery(string command)
        {
            CommandQueue.Enqueue(command);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Insert(EntityBase entity)
        {
            CommandQueue.Enqueue(SqlWriter.ConvertToInsertCommand(entity, _style));
        }

        /// <summary>
        /// Inserts the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void Insert(IEnumerable<EntityBase> entities)
        {
            foreach (var entity in entities)
            {
                CommandQueue.Enqueue(SqlWriter.ConvertToInsertCommand(entity, _style));
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(EntityBase entity)
        {
            CommandQueue.Enqueue(SqlWriter.ConvertToDeleteCommand(entity, _style));
        }

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void Delete(IEnumerable<EntityBase> entities)
        {
            foreach (var entity in entities)
            {
                CommandQueue.Enqueue(SqlWriter.ConvertToDeleteCommand(entity, _style));
            }
        }

        /// <summary>
        /// Commits or save changes made on this open connection.
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Protected Member

        protected Queue<string> CommandQueue = new Queue<string>();

        /// <summary>
        /// Raises the error event.
        /// </summary>
        /// <param name="args">
        /// The <see cref="DbExceptionEventArgs"/> instance containing the event data.
        /// </param>
        protected void RaiseError(DbExceptionEventArgs args)
        {
            Error?.Invoke(this, args);
        }

        /// <summary>
        /// Fetches the query results from the connected database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        protected abstract IEnumerable<TEntity> FetchData<TEntity>(string query);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
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