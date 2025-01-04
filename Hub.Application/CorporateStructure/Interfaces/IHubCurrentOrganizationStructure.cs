using Hub.Infrastructure.Database.Models;

namespace Hub.Application.CorporateStructure.Interfaces
{
    public interface IHubCurrentOrganizationStructure
    {
        string GetCurrentDomain(string structId = null);

        OrganizationalStructure GetCurrentRoot();

        long? GetCurrentRootId();

        List<long> UpdateUser(long userid);

        string Get();

        void Set(string id);

        void SetCookieRequest(string id);
    }
}
