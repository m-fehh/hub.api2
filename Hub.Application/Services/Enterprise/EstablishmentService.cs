using Hub.Domain.Entities.Enterprise;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services.Enterprise
{
    public class EstablishmentService : OrchestratorService<Establishment>
    {
        public EstablishmentService(IRepository<Establishment> repository) : base(repository) { }
        
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
