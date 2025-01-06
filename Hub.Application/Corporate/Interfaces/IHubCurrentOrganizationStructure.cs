using Hub.Infrastructure.Database.Models.Helpers;
using Hub.Infrastructure.Database.Models.Tenant;

namespace Hub.Application.Corporate.Interfaces
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

    /// <summary>
    /// Interface usada pela plataforma para identificar qual o atual nível organizacional selecionado pelo usuário.
    /// Para sistemas satélites (ex: app), não se aplica.
    /// </summary>
    public interface ICurrentOrganizationStructure
    {
        OrganizationalStructureVM GetCurrentDomain(long? structId = null);

        OrganizationalStructureVM GetCurrentRoot();

        OrganizationalStructureVM GetCurrent();

        OrganizationalStructureVM Set(long id);

        void Set(OrganizationalStructureVM org);

        void SetByCookieData(string cookieData);

        //TimeZoneInfo GetCurrentTimezone();
    }
}
