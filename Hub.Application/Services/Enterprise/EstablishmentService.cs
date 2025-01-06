using Hub.Application.Corporate.Handler;
using Hub.Application.Models.Helpers.Incorporation;
using Hub.Application.Services.Enterprise.Incorporation;
using Hub.Domain.Entities.Enterprise;
using Hub.Domain.Entities.Enterprise.Incorporation;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services.Enterprise
{
    public class EstablishmentService : OrchestratorService<Establishment>
    {
        public EstablishmentService(IRepository<Establishment> repository) : base(repository) { }

        #region IMPERSONATE ESTABLISHMENT  

        public void IncorporeEstablishment(Establishment entity, IncorporationEstablishmentResult incorporationEstablishmentResult)
        {
            if (incorporationEstablishmentResult.CreateIncorporationRecord)
            {
                var currentUser = Engine.Resolve<ISecurityProvider>().GetCurrentId().Value;

                var incorporationEstablishment = new IncorporationEstablishment
                {
                    CNPJ = incorporationEstablishmentResult.CNPJ,
                    Establishment = entity,
                    IncorporationDate = DateTime.Now,
                    OrganizationalStructure = entity.OrganizationalStructure,
                    User = new PortalUser { Id = currentUser }
                };
                var id = Engine.Resolve<IncorporationEstablishmentService>().Insert(incorporationEstablishment);
                incorporationEstablishment.Id = id;

                IncorporeEstablishmentConfig(new IncorporationEstablishmentHandler
                {
                    OrganizationalStructure = entity.OrganizationalStructure,
                    IncorporationEstablishment = incorporationEstablishment,
                    ConfigValue = Engine.Resolve<OrganizationalStructureService>().GetConfigByName(entity.OrganizationalStructure, "GatewayELOSAPIKey")
                });
            }
        }

        public void IncorporeEstablishmentConfig(IncorporationEstablishmentHandler incorporationEstablishmentDTO)
        {
            var incorporationEstablishmentConfig = Engine.Resolve<IncorporationEstablishmentConfigService>()
                .Get(w => w.IncorporationEstablishment.Id == incorporationEstablishmentDTO.IncorporationEstablishment.Id, s => new IncorporationEstablishmentConfig
                {
                    Id = s.Id,
                    Config = s.Config,
                    IncorporationEstablishment = s.IncorporationEstablishment,
                    OrganizationalStructure = s.OrganizationalStructure,
                    Value = s.Value
                }).FirstOrDefault();

            if (incorporationEstablishmentConfig == null)
            {
                Engine.Resolve<IncorporationEstablishmentConfigService>().Insert(new IncorporationEstablishmentConfig
                {
                    IncorporationEstablishment = incorporationEstablishmentDTO.IncorporationEstablishment,
                    OrganizationalStructure = incorporationEstablishmentDTO.OrganizationalStructure,
                    Config = incorporationEstablishmentDTO.Config,
                    Value = incorporationEstablishmentDTO.ConfigValue
                });
            }
            else
            {
                incorporationEstablishmentConfig.Value = incorporationEstablishmentDTO.ConfigValue;
                incorporationEstablishmentConfig.Config = incorporationEstablishmentDTO.Config;
                incorporationEstablishmentConfig.IncorporationEstablishment = incorporationEstablishmentDTO.IncorporationEstablishment;
                incorporationEstablishmentConfig.OrganizationalStructure = incorporationEstablishmentDTO.OrganizationalStructure;

                Engine.Resolve<IncorporationEstablishmentConfigService>().Update(incorporationEstablishmentConfig);
            }
        }


        #endregion

        #region TIMEZONE   

        public int GetEstablishmentOffset(long orgId, DateTime? baseDate = null)
        {
            var timeZone = Table.Where(c => c.OrganizationalStructure.Id == orgId).Select(s => s.TimezoneIdentifier).FirstOrDefault();
            return GetOffset(timeZone);
        }

        public int GetOffset(string timeZone, DateTime? baseDate = null)
        {
            if (!string.IsNullOrEmpty(timeZone))
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

                if (baseDate == null) baseDate = DateTime.Now;

                var utcOffset = new DateTimeOffset(baseDate.Value);

                return tz.GetUtcOffset(utcOffset).Hours;
            }
            else
            {
                return 0;
            }
        }

        public int GetEstablishmentOffset(Establishment estab, DateTime? baseDate = null)
        {
            if (!string.IsNullOrEmpty(estab.TimezoneIdentifier))
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(estab.TimezoneIdentifier);

                if (baseDate == null) baseDate = DateTime.Now;

                var utcOffset = new DateTimeOffset(baseDate.Value);

                return (tz.GetUtcOffset(utcOffset) - TimeZoneInfo.Local.GetUtcOffset(utcOffset)).Hours;
            }
            else
            {
                return 0;
            }
        } 

        #endregion

        #region PRIVATE METHODS

        private void Validate(Establishment entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.CommercialName) && _repository.Table.Where(s => entity.CommercialName.Equals(s.CommercialName) && s.Id != entity.Id).Select(s => s.Id).Any())
            {
                throw new BusinessException(Engine.Get("CommercialNameAlreadyExists"));
            }

            if (entity.CommercialAbbrev?.Length > 15)
            {
                throw new BusinessException(string.Format(Engine.Get("generic_maxlength_message"), Engine.Get("CommercialAbbrev"), "15"));
            }
        }

        private void ValidateInsert(Establishment entity)
        {
            Validate(entity);
        }

        #endregion
    }
}
