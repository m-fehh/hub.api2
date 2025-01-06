using Hub.Infrastructure.Database.Models.Tenant;

namespace Hub.Infrastructure.Database.Interfaces
{
    public interface IEntityOrgStructOwned
    {
        OrganizationalStructure? OwnerOrgStruct { get; set; }
    }
}
