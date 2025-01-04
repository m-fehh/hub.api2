using Hub.Infrastructure.ViewModels;

namespace Hub.Infrastructure.Database.Interfaces
{
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
