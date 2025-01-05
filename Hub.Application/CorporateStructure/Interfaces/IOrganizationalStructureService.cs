using Hub.Domain.Entities;
using Hub.Infrastructure.Database.Models;
using Hub.Infrastructure.ViewModels;

namespace Hub.Application.CorporateStructure.Interfaces
{
    public interface IOrganizationalStructureService : IOrgStructBasedService
    {
        bool IsLeafStructure(long? structId = null);

        bool IsDomainStructure(long? structId = null);

        bool IsRootStructure(long? structId = null);

        string GetConfigByName(OrganizationalStructure org, string name);

        string GetConfigByName(long? orgId_, string name_);

        void SetConfig(OrganizationalStructure org, string name, string value);

        string GetCurrentConfigByName(string name);

        bool ClientUseNickName(long structureId);

        List<OrganizationalStructureTreeModelVM> GenerateTreeList(bool removeNotAccessOrg = true);

        Establishment GetCurrentEstablishment(string currentStringLevel = null);
    }
}
