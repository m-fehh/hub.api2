using Hub.Infrastructure.Autofac.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;

namespace Hub.Infrastructure.Database.Interfaces
{
    public interface IRepository<T> : IRepository where T : IBaseEntity
    {
        long Insert(T entity);

        long StatelessInsert(T entity);

        void Update(T entity);

        void StatelessUpdate(T entity);

        void Delete(long id);

        void Delete(T entity);

        void Refresh(T entity);

        T GetById(long id);

        T StatelessGetById(long id);

        IQueryable<T> Table { get; }

    }

    public interface IRepository : ISetType
    {
        IDisposable BeginTransaction();

        void Commit();

        void RollBack();

        void Flush();

        void Clear();

        bool IsInitialized(object entity);
    }
}
