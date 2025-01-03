using Hub.Domain.Entities.OrgStructure;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services
{
    public class OrganizationalStructureService : CrudService<OrganizationalStructure>
    {
        private static object locker = new object();
        private static long? currentOrgStructureIfNull;

        public OrganizationalStructureService(IRepository<OrganizationalStructure> repository) : base(repository) { }
    }
}