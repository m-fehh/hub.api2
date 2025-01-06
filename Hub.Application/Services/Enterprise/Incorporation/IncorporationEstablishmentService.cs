using Hub.Application.Models.Helpers.Incorporation;
using Hub.Domain.Entities.Enterprise.Incorporation;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services.Enterprise.Incorporation
{
    public class IncorporationEstablishmentService : OrchestratorService<IncorporationEstablishment>
    {
        public IncorporationEstablishmentService(IRepository<IncorporationEstablishment> repository) : base(repository) { }

        private void Validate(IncorporationEstablishment entity)
        {
            if (entity.Establishment == null)
            {
                throw new BusinessException(Engine.Get("EstablishmentIsRequired"));
            }

            if (entity.OrganizationalStructure == null)
            {
                throw new BusinessException(Engine.Get("OrganizationalStructureIsRequired"));
            }

            if (string.IsNullOrWhiteSpace(entity.CNPJ))
            {
                throw new BusinessException(Engine.Get("CNPJIsRequired"));
            }
        }

        private void ValidateInsert(IncorporationEstablishment entity)
        {
            Validate(entity);
        }

        public override long Insert(IncorporationEstablishment entity)
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

        public override void Update(IncorporationEstablishment entity)
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

        /// <summary>
        /// Método responsável por validar se permite incorporar o estabelecimento
        /// </summary>
        /// <param name="filter">Classe com dados do estabelecimento, Id e CNPJ</param>
        /// <returns>Verdadeiro se pode incorporar o estabelecimento e falso caso não possa incorporar o estabelecimento</returns>
        public IncorporationEstablishmentResult AllowsCreateIncorporationRecord(IncorporationEstablishmentFilter filter)
        {
            var cnpj = Engine.Resolve<EstablishmentService>().Table.Where(w => w.Id == filter.EstablishmentId).Select(s => s.CNPJ).FirstOrDefault();
            return new IncorporationEstablishmentResult
            {
                CreateIncorporationRecord = !string.IsNullOrWhiteSpace(cnpj) && cnpj != filter.CNPJ,
                CNPJ = cnpj
            };
        }

        /// <summary>
        /// Método responsável por retornas as incorporações do estabelecimento
        /// </summary>
        /// <param name="establishmentId">Id do estabelecimento</param>
        /// <returns>Incorporações do estabelecimento</returns>
        public IncorporationEstablishment GetByEstablishmentId(IncorporationEstablishmentFilter filter)
        {
            var incorporationEstablishment = Engine.Resolve<IncorporationEstablishmentService>().Get(w => w.Establishment.Id == filter.EstablishmentId && w.CNPJ == filter.CNPJ,
                    s => new IncorporationEstablishment
                    {
                        Id = s.Id,
                        CNPJ = s.CNPJ,
                        Establishment = s.Establishment,
                        IncorporationDate = s.IncorporationDate,
                        OrganizationalStructure = s.OrganizationalStructure
                    }).FirstOrDefault();

            return incorporationEstablishment;
        }
    }
}
