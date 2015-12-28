using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Data
{
    public interface IDbConnectionWrapper : System.IDisposable
    {
        IEnumerable<TEntity> Execute<TEntity>(string query);

        IEnumerable<TEntity> Execute<TEntity>(Linq.Query query);

        void ExecuteNonQuery(string command);

        void Insert(EntityBase entity);

        void Insert(IEnumerable<EntityBase> entities);

        void Delete(EntityBase entity);

        void Delete(IEnumerable<EntityBase> entities);

        void Commit();
    }
}