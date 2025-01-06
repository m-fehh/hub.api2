using Hub.Infrastructure.Database.Models.Tenant;

namespace Hub.Infrastructure.Database.Interfaces
{
    public interface IEntityOrgStructBased
    {
        ICollection<OrganizationalStructure>? OrganizationalStructures { get; set; }
    }
}
