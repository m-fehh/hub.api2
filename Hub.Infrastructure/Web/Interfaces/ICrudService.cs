using Hub.Infrastructure.Database.Entity.Interfaces;
using System.Linq.Expressions;

namespace Hub.Infrastructure.Web.Interfaces
{
    public interface ICrudService<T> : ICrudService where T : IBaseEntity
    {
        T GetById(long id);

        long Insert(T entity);

        void Update(T entity);

        IQueryable<T> Table { get; }

        IQueryable<TResult> Get<TResult>(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, TResult>> selectPredicate, int quantity = 0, bool useCacheableTable = false);
    }

    public interface ICrudService
    {
        void Delete(long id);
    }

    //public interface ICrudService<T, M> : ICrudService<T> where T : IBaseEntity where M : ICrudModel
    //{
    //    T Insert(M model);

    //    T Update(M model);
    //}
}
