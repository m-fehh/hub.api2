using Hub.Domain.Entities.Enterprise.Incorporation;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Application.Services.Enterprise.Incorporation
{
    public class IncorporationEstablishmentConfigService : OrchestratorService<IncorporationEstablishmentConfig>
    {
        public IncorporationEstablishmentConfigService(IRepository<IncorporationEstablishmentConfig> repository) : base(repository) { }

        private void Validate(IncorporationEstablishmentConfig entity)
        {
            if (entity.IncorporationEstablishment == null)
            {
                throw new BusinessException(Engine.Get("EstablishmentIsRequired"));
            }

            if (entity.OrganizationalStructure == null)
            {
                throw new BusinessException(Engine.Get("OrganizationalStructureIsRequired"));
            }

            if (entity.Config == null)
            {
                throw new BusinessException(Engine.Get("OrganizationalStructureConfigIsRequired"));
            }

            if (string.IsNullOrWhiteSpace(entity.Value))
            {
                throw new BusinessException(Engine.Get("ValueIsRequired"));
            }
        }

        private void ValidateInsert(IncorporationEstablishmentConfig entity)
        {
            Validate(entity);
        }

        public override long Insert(IncorporationEstablishmentConfig entity)
        {
            ValidateInsert(entity);

            long ret = 0;

            using (var transaction = base._repository.BeginTransaction())
            {
                ret = base._repository.Insert(entity);

                if (transaction != null) base._repository.Commit();
            }

            return ret;
        }

        public override void Update(IncorporationEstablishmentConfig entity)
        {
            Validate(entity);

            using (var transaction = base._repository.BeginTransaction())
            {
                base._repository.Update(entity);

                if (transaction != null) base._repository.Commit();
            }
        }

        public override void Delete(long id)
        {
            using (var transaction = base._repository.BeginTransaction())
            {
                var entity = GetById(id);

                base._repository.Delete(id);

                if (transaction != null) base._repository.Commit();
            }
        }
    }
}
