using Hub.Domain.Entities.Enterprise;
using Hub.Domain.Entities.Enterprise.Incorporation;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models.Tenant;

namespace Hub.Application.Corporate.Handler
{
    public class IncorporationEstablishmentHandler
    {
        public OrganizationalStructure OrganizationalStructure { get; set; }

        public IncorporationEstablishment IncorporationEstablishment { get; set; }

        public OrgStructConfigDefault Config
        {
            get
            {
                return new OrgStructConfigDefault
                {
                    Id = Engine.Resolve<IRepository<OrgStructConfigDefault>>().Table.Where(w => w.Name == "GatewayELOSAPIKey").Select(s => s.Id).FirstOrDefault()
                };
            }
        }

        public string ConfigValue { get; set; }
    }
}
