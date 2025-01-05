using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Hub.Infrastructure.Architecture.Logger;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web.Services;

namespace Hub.Application.Services
{
    public class EstablishmentService : OrchestratorService<Establishment>
    {
        public EstablishmentService(IRepository<Establishment> repository) : base(repository) {  }

        private void Validate(Establishment entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.CommercialName) && _repository.Table.Where(s => entity.CommercialName.Equals(s.CommercialName) && s.Id != entity.Id).Select(s => s.Id).Any())
            {
                throw new BusinessException(Engine.Get("CommercialNameAlreadyExists"));
            }
        }


        public override long Insert(Establishment entity)
        {
            Validate(entity);

            var profile = Engine.Resolve<ISecurityProvider>().GetCurrentProfile();

            var isAdmin = profile?.Administrator ?? false;

            if (!isAdmin) entity.SystemStartDate = null;

            long ret = 0;

            using (var transaction = base._repository.BeginTransaction())
            {
                Engine.Resolve<IRedisService>().Set($"TimeZone{entity.OrganizationalStructure.Id}", entity.Timezone);

                ret = base._repository.Insert(entity);

                ProcessEstablishmentTimezoneOffset(entity);

                Engine.Resolve<OrganizationalStructureService>().UpdateLastUpdateUTC(entity.OrganizationalStructureId);

                if (transaction != null) base._repository.Commit();
            }

            return ret;
        }

        public override void Update(Establishment entity)
        {
            //var isAdmin = Engine.Resolve<ISecurityProvider>().GetCurrentProfile()?.Administrator ?? false;

            //if (!isAdmin)
            //{
            //    var bife = Table.Where(e => e.Id == entity.Id).Select(e => new
            //    {
            //        e.SystemStartDate,
            //        e.EstablishmentClassifier
            //    }).FirstOrDefault();

            //    entity.SystemStartDate = bife.SystemStartDate;
            //    entity.EstablishmentClassifier = bife.EstablishmentClassifier;
            //}



            var incorporationEstablishmentResult = Engine.Resolve<IncorporationEstablishmentService>().AllowsCreateIncorporationRecord(new IncorporationEstablishmentFilter
            {
                EstablishmentId = entity.Id,
                CNPJ = entity.CNPJ
            });

            using (var transaction = base._repository.BeginTransaction())
            {
                Engine.Resolve<IRedisService>().Set($"TimeZone{entity.OrganizationalStructure.Id}", entity.Timezone);

                EstablishmentLog(entity);
                IncorporeEstablishment(entity, incorporationEstablishmentResult);

                UpdateWithoutLogEstablishment(entity);

                ProcessEstablishmentTimezoneOffset(entity);

                Engine.Resolve<OrganizationalStructureService>().UpdateLastUpdateUTC(entity.OrganizationalStructureId);

                if (transaction != null) base._repository.Commit();
            }
        }

        public void EstablishmentLog(Establishment entity)
        {
            //var listmessage = ValidateOldValueEstablishment(oldData, entity);

            //foreach (var message in listmessage)
            //{
            //    Engine.Resolve<LogService>().Audit(Engine.Get("Establishment"), entity.Id, ELogAction.Update, 0, "", "", message);
            //}

        }

        public void ProcessEstablishmentTimezoneOffset(Establishment entity)
        {
            List<Establishment> estabs;

            if (entity == null)
            {
                estabs = _repository.Table.Where(e => !e.OrganizationalStructure.Inactive && e.Timezone != null).ToList();
            }
            else
            {
                estabs = new List<Establishment>() { entity };
            }

            using (var transaction = base._repository.BeginTransaction())
            {
                foreach (var estab in estabs)
                {
                    estab.TimezoneOffset = GetEstablishmentOffset(estab);

                    UpdateWithoutLogEstablishment(estab);
                }

                if (transaction != null) _repository.Commit();
            }
        }

        public int GetEstablishmentOffset(Establishment estab, DateTime? baseDate = null)
        {
            if (!string.IsNullOrEmpty(estab.Timezone))
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(estab.Timezone);

                if (baseDate == null) baseDate = DateTime.Now;

                var utcOffset = new DateTimeOffset(baseDate.Value);

                return (tz.GetUtcOffset(utcOffset) - TimeZoneInfo.Local.GetUtcOffset(utcOffset)).Hours;
            }
            else
            {
                return 0;
            }
        }

        public void UpdateWithoutLogEstablishment(Establishment entity)
        {
            using (Engine.Resolve<IgnoreLogScope>().BeginIgnore())
            {
                base._repository.Update(entity);
            }
        }
    }
}
