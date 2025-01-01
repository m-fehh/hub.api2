using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Logger;
using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Architecture.Logger.Interfaces;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hub.Infrastructure.Database
{
    public class Repository<T> : Repository, IRepository<T> where T : class, IBaseEntity
    {
        public Repository(DbContext context) : base(context) { }

        public Repository(DbContext context, string tenantName) : base(context, tenantName) { }

        public T GetById(long id)
        {
            return _context.Set<T>().Find(id);
        }

        public T StatelessGetById(long id)
        {
            return _context.Set<T>().AsNoTracking().FirstOrDefault(x => EF.Property<long>(x, "Id") == id);
        }

        public long Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (_context.Database.CurrentTransaction == null)
                throw new InvalidOperationException("Nenhuma transação ativa");

            if (entity is IModificationControl)
            {
                if (!Engine.Resolve<IgnoreModificationControl>().Ignore)
                {
                    (entity as IModificationControl).CreationUTC = DateTime.UtcNow;
                }
            }

            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            Log(entity, ELogAction.Insertion);

            return (long)typeof(T).GetProperty("Id")?.GetValue(entity);
        }

        public long StatelessInsert(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (_context.Database.CurrentTransaction == null)
                throw new InvalidOperationException("Nenhuma transação ativa");

            if (entity is IModificationControl)
            {
                if (!Engine.Resolve<IgnoreModificationControl>().Ignore)
                {
                    (entity as IModificationControl).CreationUTC = DateTime.UtcNow;
                }
            }

            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return (long)typeof(T).GetProperty("Id")?.GetValue(entity);
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (_context.Database.CurrentTransaction == null)
                throw new InvalidOperationException("Nenhuma transação ativa");

            if (entity is IModificationControl)
            {
                if (!Engine.Resolve<IgnoreModificationControl>().Ignore)
                {
                    (entity as IModificationControl).LastUpdateUTC = DateTime.UtcNow;
                }
            }

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            Log(entity, ELogAction.Update);
        }

        public void StatelessUpdate(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (_context.Database.CurrentTransaction == null)
                throw new InvalidOperationException("Nenhuma transação ativa");

            if (entity is IModificationControl)
            {
                if (!Engine.Resolve<IgnoreModificationControl>().Ignore)
                {
                    (entity as IModificationControl).LastUpdateUTC = DateTime.UtcNow;
                }
            }

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            Log(entity, ELogAction.Update);
        }

        public void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (_context.Database.CurrentTransaction == null)
                throw new InvalidOperationException("Nenhuma transação ativa");

            _context.Set<T>().Remove(entity);
            _context.SaveChanges();

            Log(entity, ELogAction.Deletion);
        }

        public void Refresh(T entity)
        {
            _context.Entry(entity).Reload();
        }

        public void Delete(long id)
        {
            var entity = _context.Set<T>().Find(id);

            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();

                Log(entity, ELogAction.Deletion);
            }
            else
            {
                throw new InvalidOperationException($"Entity with id {id} not found");
            }
        }

        public IQueryable<T> Table
        {
            get
            {
                return IncludeAll(_context.Set<T>());
            }
        }

        public IQueryable<T> IncludeAll<T>(IQueryable<T> query) where T : class
        {
            var entityType = _context.Model.FindEntityType(typeof(T));

            var navigationProperties = entityType.GetNavigations();

            // Itera sobre todas as propriedades de navegação e aplica o Include
            foreach (var navigation in navigationProperties)
            {
                query = query.Include(navigation.Name);
            }

            return query;
        }

        private void Log(T entity, ELogAction action)
        {
            if (Engine.Resolve<IgnoreLogScope>().Ignore) return;

            if (_logManager == null) Engine.TryResolve(out _logManager);

            if (_logManager != null)
            {
                var log = _logManager.Audit(entity, action, true, action == ELogAction.Update);
                if (log != null) _context.Set<ILog>().Add(log as ILog);
            }
        }
    }

    public class Repository : IRepository
    {
        protected DbContext _context;
        protected string tenantName;
        protected ILogManager _logManager;

        protected Type _resolvedType;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public Repository(DbContext context, string tenantName) : this(context)
        {
            this.tenantName = tenantName;
        }

        public IDisposable BeginTransaction()
        {
            if (_context.Database.CurrentTransaction != null) return null;

            return _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                _context.Database.CommitTransaction();
            }
        }

        public void RollBack()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                _context.Database.RollbackTransaction();
            }
        }

        public void Flush()
        {
            _context.SaveChanges();
        }

        public void Clear()
        {
            // Clear context cache
            _context.ChangeTracker.Clear();
        }

        public void Evict(object entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        // Verifica se a entidade foi inicializada
        public bool IsInitialized(object entity)
        {
            return _context.Entry(entity).State != EntityState.Detached;
        }

        #region ISetType Members

        /// <summary>
        /// Ao utiliar o repositório com uma interface como parametro, o método SetType será invocado pelo Autofac para passar o tipo resolvido da interface, 
        /// e então todos os demais métodos do repositório deverão considerá-la para não passar uma interface para a sessão
        /// </summary>
        /// <param name="resolvedType"></param>
        public void SetType(Type resolvedType)
        {
            _resolvedType = resolvedType;
        }

        #endregion
    }
}
