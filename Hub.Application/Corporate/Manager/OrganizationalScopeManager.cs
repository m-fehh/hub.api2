using Hub.Domain.Enums;

namespace Hub.Application.Corporate.Manager
{
    public class OrganizationalScopeManager
    {
        public EOrganizationalHierarchyLevel CurrentScope { get; set; }

        public long? CurrentUser { get; set; }
    }
}
