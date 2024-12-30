//using Hub.Domain.Common.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Storage;
//using System.Data;

//namespace Hub.Domain.Common
//{
//    public class Repository<T> : Repository, IRepository<T> where T : class, IBaseEntity
//    {
//        private readonly DbContext _context;

//        public Repository(DbContext context) : base(context)
//        {
//            _context = context;
//        }

//        // Insere uma nova entidade
//        public long Insert(T entity)
//        {
//            if (entity == null) throw new ArgumentNullException(nameof(entity));
//            _context.Set<T>().Add(entity);
//            _context.SaveChanges();
//            return (long)typeof(T).GetProperty("Id")?.GetValue(entity);
//        }

//        // Insere uma nova entidade sem manter o estado da transação
//        public long StatelessInsert(T entity)
//        {
//            if (entity == null) throw new ArgumentNullException(nameof(entity));
//            _context.Set<T>().Add(entity);
//            _context.SaveChanges();
//            return (long)typeof(T).GetProperty("Id")?.GetValue(entity);
//        }

//        // Atualiza uma entidade existente
//        public void Update(T entity)
//        {
//            if (entity == null) throw new ArgumentNullException(nameof(entity));
//            _context.Set<T>().Update(entity);
//            _context.SaveChanges();
//        }

//        // Atualiza uma entidade sem manter o estado da transação
//        public void StatelessUpdate(T entity)
//        {
//            if (entity == null) throw new ArgumentNullException(nameof(entity));
//            _context.Set<T>().Update(entity);
//            _context.SaveChanges();
//        }

//        // Deleta uma entidade por ID
//        public void Delete(long id)
//        {
//            var entity = _context.Set<T>().Find(id);
//            if (entity != null)
//            {
//                _context.Set<T>().Remove(entity);
//                _context.SaveChanges();
//            }
//            else
//            {
//                throw new InvalidOperationException($"Entity with id {id} not found");
//            }
//        }

//        // Deleta uma entidade específica
//        public void Delete(T entity)
//        {
//            if (entity == null) throw new ArgumentNullException(nameof(entity));
//            _context.Set<T>().Remove(entity);
//            _context.SaveChanges();
//        }

//        // Recupera uma entidade por ID
//        public T GetById(long id)
//        {
//            return _context.Set<T>().Find(id);
//        }

//        // Recupera uma entidade sem estado por ID
//        public T StatelessGetById(long id)
//        {
//            // Stateless retrieval logic, e.g., no tracking
//            return _context.Set<T>().AsNoTracking().FirstOrDefault(x => EF.Property<long>(x, "Id") == id);
//        }

//        // Propriedade para acessar a tabela de entidades com rastreamento
//        public IQueryable<T> Table => _context.Set<T>();

//        // Propriedade para acessar a tabela de entidades sem rastreamento
//        public IQueryable<T> StatelessTable => _context.Set<T>().AsNoTracking();
//    }

//    public class Repository : IRepository
//    {
//        protected string tenantName;
//        private IDbContextTransaction _currentTransaction;
//        protected Type _resolvedType;

//        private readonly DbContext _context;
//        public Repository(DbContext context)
//        {
//            _context = context;
//        }

//        // Inicia uma nova transação
//        public IDisposable BeginTransaction()
//        {
//            if (_currentTransaction != null)
//                throw new InvalidOperationException("Transaction already started");

//            _currentTransaction = _context.Database.BeginTransaction();
//            return _currentTransaction;
//        }

//        // Inicia uma nova transação com nível de isolamento
//        public IDisposable BeginTransaction(IsolationLevel isolationLevel)
//        {
//            if (_currentTransaction != null)
//                throw new InvalidOperationException("Transaction already started");

//            _currentTransaction = _context.Database.BeginTransaction(isolationLevel);
//            return _currentTransaction;
//        }

//        // Inicia uma transação sem estado
//        public IDisposable BeginStatelessTransaction()
//        {
//            // Stateless logic here if applicable
//            return BeginTransaction();
//        }

//        // Commit da transação
//        public void Commit()
//        {
//            if (_currentTransaction == null)
//                throw new InvalidOperationException("Transaction has not started");

//            _currentTransaction.Commit();
//            _currentTransaction.Dispose();
//            _currentTransaction = null;
//        }

//        // Commit da transação sem estado
//        public void CommitStateless()
//        {
//            // Stateless commit logic, if any
//            Commit();
//        }

//        // Rollback da transação
//        public void RollBack()
//        {
//            if (_currentTransaction == null)
//                throw new InvalidOperationException("Transaction has not started");

//            _currentTransaction.Rollback();
//            _currentTransaction.Dispose();
//            _currentTransaction = null;
//        }

//        #region ISetType Members

//        /// <summary>
//        /// Ao utilizar o repositório com uma interface como parâmetro, o método SetType será invocado pelo Autofac para passar o tipo resolvido da interface,
//        /// e então todos os demais métodos do repositório deverão considerá-la para não passar uma interface para a sessão
//        /// </summary>
//        /// <param name="resolvedType"></param>
//        public void SetType(Type resolvedType)
//        {
//            _resolvedType = resolvedType;
//        }

//        #endregion


//        // Limpa o contexto do EF
//        public void Clear()
//        {
//            _context.ChangeTracker.Clear();
//        }

//        // Executa o flush no contexto
//        public void Flush()
//        {
//            _context.SaveChanges();
//        }

//        // Verifica se a entidade foi inicializada
//        public bool IsInitialized(object entity)
//        {
//            return _context.Entry(entity).State != EntityState.Detached;
//        }
//    }
//}


using Hub.Infrastructure;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Logger;
using Hub.Infrastructure.Logger.Enums;
using Hub.Infrastructure.Logger.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hub.Infrastructure.Database
{
    public class Repository<T> : Repository, IRepository<T> where T : class, IBaseEntity
    {
        public Repository(DbContext context) : base(context)
        {
        }

        public Repository(DbContext context, string tenantName) : base(context, tenantName)
        {
        }

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
                    (entity as IModificationControl).LastUpdateUTC = DateTime.UtcNow;
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
                    (entity as IModificationControl).LastUpdateUTC = DateTime.UtcNow;
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
                return _context.Set<T>();
            }
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
