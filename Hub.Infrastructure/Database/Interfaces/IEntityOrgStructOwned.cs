using Hub.Infrastructure.Database.Models;

namespace Hub.Infrastructure.Database.Interfaces
{
    public interface IEntityOrgStructOwned
    {
        OrganizationalStructure OwnerOrgStruct { get; set; }
    }

    public interface IEntityOrgStructBased
    {
        ICollection<OrganizationalStructure> OrganizationalStructures { get; set; }
    }
}
