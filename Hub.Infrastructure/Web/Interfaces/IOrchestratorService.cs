using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;
using System.Linq.Expressions;

namespace Hub.Infrastructure.Web.Interfaces
{
    public interface IOrchestratorService<T> : IOrchestratorService where T : IBaseEntity
    {
        T GetById(long id);

        long Insert(T entity);

        void Update(T entity);

        IQueryable<T> Table { get; }

        IQueryable<TResult> Get<TResult>(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, TResult>> selectPredicate, int quantity = 0, bool useCacheableTable = false);
    }

    public interface IOrchestratorService
    {
        void Delete(long id);
    }

    public interface IOrchestratorService<T, M> : IOrchestratorService<T> where T : IBaseEntity where M : IModelEntity
    {
        T Insert(M model);
        T Update(M model);
    }
}
