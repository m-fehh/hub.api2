using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web.Interfaces;
using System.Linq.Expressions;

namespace Hub.Infrastructure.Web
{
    /// <summary>
    /// Implementação básica do serviço de CRUD. Essa classe pode ser usada para facilitar a implementação da interface <see cref="ICrudService"/>
    /// </summary>
    /// <typeparam name="T">Entidade cuidada pela crud dessa classe</typeparam>
    public class OrchestratorService<T> : IOrchestratorService<T> where T : class, IBaseEntity
    {
        protected readonly IRepository<T> _repository;

        public OrchestratorService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual T GetById(long id)
        {
            return _repository.GetById(id);
        }

        public virtual long Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Table
        {
            get { return _repository.Table; }
        }


        public IQueryable<TResult> Get<TResult>(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, TResult>> selectPredicate, int quantity = 0, bool useCacheableTable = false)
        {
            var query = _repository.Table.Where(wherePredicate).Select(selectPredicate);

            if (quantity > 0)
                query = query.Take(quantity);

            return query;
        }
    }

    public class OrchestratorService<T, M> : OrchestratorService<T>, IOrchestratorService<T, M> where T : class, IBaseEntity where M : IModelEntity
    {
        protected IDataMapper<T, M> modelMapper;

        public OrchestratorService(IRepository<T> repository)
            : base(repository)
        {
            modelMapper = Engine.Resolve<IDataMapper<T, M>>();
        }

        public virtual T Insert(M model)
        {
            throw new NotImplementedException();
        }

        public virtual T Update(M model)
        {
            throw new NotImplementedException();
        }
    }
}
