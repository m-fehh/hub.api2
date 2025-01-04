using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web.Services;

namespace Hub.Application.Services
{
    public class PortalUserPassHistoryService : OrchestratorService<PortalUserPassHistory>
    {
        public PortalUserPassHistoryService(IRepository<PortalUserPassHistory> repository) : base(repository) { }

        private void Validate(PortalUserPassHistory entity) { }

        private void ValidateInsert(PortalUserPassHistory entity)
        {
            Validate(entity);
        }

        public override long Insert(PortalUserPassHistory entity)
        {
            ValidateInsert(entity);

            using (var transaction = _repository.BeginTransaction())
            {
                var ret =   _repository.Insert(entity);

                if (transaction != null) _repository.Commit();

                return ret;
            }
        }

        public override void Update(PortalUserPassHistory entity)
        {
            Validate(entity);

            using (var transaction = _repository.BeginTransaction())
            {
                _repository.Update(entity);

                if (transaction != null) _repository.Commit();
            }
        }

        public override void Delete(long id)
        {
            using (var transaction = _repository.BeginTransaction())
            {
                _repository.Delete(id);

                if (transaction != null) _repository.Commit();
            }
        }
    }
}
