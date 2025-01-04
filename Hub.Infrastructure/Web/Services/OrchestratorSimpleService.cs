using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;

namespace Hub.Infrastructure.Web.Services
{
    /// <summary>
    /// Implementação básica do serviço de CRUD. Essa classe pode ser usada para facilitar a implementação da interface <see cref="ICrudService"/>
    /// </summary>
    /// <typeparam name="T">Entidade cuidada pela crud dessa classe</typeparam>
    public class OrchestratorSimpleService<T> : OrchestratorService<T>
        where T : class, IBaseEntity
    {
        protected event EventHandler<T> OnBeforeInsert;
        protected event EventHandler<T> OnBeforeUpdate;
        protected event EventHandler<long> OnBeforeDelete;

        public OrchestratorSimpleService(IRepository<T> repository) : base(repository)
        {
        }

        public override long Insert(T entity)
        {
            OnBeforeInsert?.Invoke(this, entity);

            using (var transaction = _repository.BeginTransaction())
            {
                var ret = _repository.Insert(entity);

                if (transaction != null) _repository.Commit();

                return ret;
            }
        }

        public override void Update(T entity)
        {
            OnBeforeUpdate?.Invoke(this, entity);

            using (var transaction = _repository.BeginTransaction())
            {
                _repository.Update(entity);

                if (transaction != null) _repository.Commit();
            }
        }

        public override void Delete(long id)
        {
            OnBeforeDelete?.Invoke(this, id);

            using (var transaction = _repository.BeginTransaction())
            {
                _repository.Delete(id);

                if (transaction != null) _repository.Commit();
            }
        }
    }
}
