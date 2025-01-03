using Hub.Domain.Enums;

namespace Hub.Application.CorporateStructure
{
    public class OrganizationalScopeManager
    {
        public EOrganizationalHierarchyLevel CurrentScope { get; set; }

        public long? CurrentUser { get; set; }
    }

    public class OrganizationalHandler
    {
        public bool RunningInTestScope { get; set; }
    }
}
