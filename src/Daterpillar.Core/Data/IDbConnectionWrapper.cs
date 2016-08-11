using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Data
{
    /// <summary>
    /// Provides a mechanism to interact with relational databases.
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public interface IDbConnectionWrapper : System.IDisposable
    {
        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        IEnumerable<TEntity> Execute<TEntity>(string query);

        IEnumerable<TEntity> Execute<TEntity>(Linq.Query query);

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="command">The command.</param>
        void ExecuteNonQuery(string command);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert(EntityBase entity);

        /// <summary>
        /// Inserts the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Insert(IEnumerable<EntityBase> entities);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(EntityBase entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Delete(IEnumerable<EntityBase> entities);

        /// <summary>
        /// Commits or save changes made on this open connection.
        /// </summary>
        void Commit();
    }
}