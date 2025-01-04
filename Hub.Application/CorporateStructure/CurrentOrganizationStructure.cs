using Hub.Domain.Enums;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.ViewModels;
using Hub.Infrastructure.Web.Interfaces;
using Hub.Infrastructure.Web;
using Newtonsoft.Json;
using Hub.Application.Services;
using Hub.Domain.Entities.Users;
using Hub.Application.CorporateStructure.Interfaces;
using Hub.Infrastructure.Database.Models;

namespace Hub.Application.CorporateStructure
{
    public class HubCurrentOrganizationStructure : IHubCurrentOrganizationStructure
    {
        public void Set(string id)
        {
            HttpContextHelper.Current.Response.Cookies.Append("current-organizational-structure", id);
        }

        public void SetCookieRequest(string id)
        {
            HttpContextHelper.Current.Request.Cookies.Append(new KeyValuePair<string, string>("current-organizational-structure", id));
        }

        public List<long> UpdateUser(long userid)
        {
            var redisService = Engine.Resolve<IRedisService>();

            var list = Engine.Resolve<IOrchestratorService<PortalUser>>().Table.Where(c => c.Id == userid).SelectMany(c => c.OrganizationalStructures).Select(o => o.Id).ToList();

            redisService.Set($"UserOrgList{userid}", JsonConvert.SerializeObject(list));

            return list;
        }

        public string Get()
        {
            try
            {
                if (Singleton<OrganizationalHandler>.Instance?.RunningInTestScope ?? false)
                {
                    var orgService = Engine.Resolve<OrganizationalStructureService>();

                    switch (Singleton<OrganizationalScopeManager>.Instance.CurrentScope)
                    {
                        case EOrganizationalHierarchyLevel.Leaf:
                            return orgService.Table.Where(c => c.IsLeaf && !c.Inactive).Select(c => c.Id).FirstOrDefault().ToString();

                        case EOrganizationalHierarchyLevel.Domain:
                            return orgService.Table.Where(c => c.IsDomain && !c.Inactive).Select(c => c.Id).FirstOrDefault().ToString();

                        case EOrganizationalHierarchyLevel.Root:
                            return orgService.Table.Where(c => c.IsRoot && !c.Inactive).Select(c => c.Id).FirstOrDefault().ToString();

                        default:
                            break;
                    }
                }

                var userId = Engine.Resolve<ISecurityProvider>().GetCurrentId();

                if (userId == null) return null;

                var cookie = HttpContextHelper.Current.Request.Cookies["current-organizational-structure"];

                if (cookie == null)
                {
                    var current = Engine.Resolve<IUserSettingManager>().GetSetting("current-organizational-structure");

                    Set(current);

                    return current;
                }

                long l;

                if (long.TryParse(cookie, out l))
                {
                    var redisService = Engine.Resolve<IRedisService>();

                    List<long> UsersOrgStructs = null;

                    var UsersOrgStructsString = redisService.Get($"UserOrgList{userId}").ToString();

                    if (!string.IsNullOrEmpty(UsersOrgStructsString))
                    {
                        UsersOrgStructs = JsonConvert.DeserializeObject<List<long>>(UsersOrgStructsString);
                    }
                    else
                    {
                        UsersOrgStructs = UpdateUser(userId.Value);
                    }

                    if (!UsersOrgStructs.Any(o => o == l))
                    {
                        var defaultOrgStructure = Engine.Resolve<IRepository<PortalUser>>().Table.Where(u => u.Id == userId.Value).Select(u => u.DefaultOrgStructureId).FirstOrDefault();

                        Set(defaultOrgStructure.ToString());

                        return defaultOrgStructure.ToString();
                    }

                    return cookie;
                }
                else
                {
                    var defaultOrgStructure = Engine.Resolve<IRepository<PortalUser>>().Table.Where(u => u.Id == userId.Value).Select(u => u.DefaultOrgStructureId).FirstOrDefault();

                    Set(defaultOrgStructure.ToString());

                    return defaultOrgStructure.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetCurrentDomain(string structId = null)
        {
            if (string.IsNullOrWhiteSpace(structId))
            {
                structId = Engine.Resolve<IHubCurrentOrganizationStructure>().Get();

                var localcached = Engine.Resolve<PortalCache>().Get().CurrentDomain;

                if (!string.IsNullOrEmpty(localcached)) return localcached;

            }

            var redisService = Engine.Resolve<IRedisService>();

            var cached = redisService.Get($"CurrentDomain{structId}").ToString();

            if (!string.IsNullOrEmpty(cached))
            {
                try
                {
                    Engine.Resolve<PortalCache>().Get().CurrentDomain = cached;
                }
                catch (Exception) { }

                return cached;
            }

            var fromDb = GetCurrentDomainFromDb(structId);

            redisService.Set($"CurrentDomain{structId}", fromDb, TimeSpan.FromHours(3));

            try
            {
                Engine.Resolve<PortalCache>().Get().CurrentDomain = fromDb;
            }
            catch (Exception) { }

            return fromDb;
        }

        private string GetCurrentDomainFromDb(string structId = null)
        {
            var structService = Engine.Resolve<IOrchestratorService<OrganizationalStructure>>();
            var longStructId = long.Parse(structId);

            var structure = structService.Table.Where(o => o.Id == longStructId).Select(o => new { o.IsDomain, FatherId = (long?)o.Father.Id }).FirstOrDefault();

            if (structure == null)
            {
                throw new Exception(Engine.Get("OrganizationalStructureNotFound"));
            }

            if (structure.IsDomain) return structId;

            if (structure.FatherId != null)
            {
                // Caso a unidade não possuir Central
                if (structService.Table.Any(c => c.Id == structure.FatherId && c.IsRoot))
                    return structId;

                return GetCurrentDomainFromDb(structure.FatherId.ToString());
            }
            else
            {
                return null;
            }
        }

        public OrganizationalStructure GetCurrentRoot()
        {
            return Engine.Resolve<IRepository<OrganizationalStructure>>().Table.FirstOrDefault(o => o.IsRoot == true);
        }

        public long? GetCurrentRootId()
        {
            return Engine.Resolve<IRepository<OrganizationalStructure>>().Table.Where(o => o.IsRoot == true).Select(o => (long?)o.Id).FirstOrDefault();
        }
    }

    public class CurrentOrganizationStructure : ICurrentOrganizationStructure
    {
        private readonly IRepository<OrganizationalStructure> repository;
        private readonly IHubCurrentOrganizationStructure hubCurrentOrganizationStructure;

        public CurrentOrganizationStructure(IRepository<OrganizationalStructure> repository, IHubCurrentOrganizationStructure hubCurrentOrganizationStructure)
        {
            this.repository = repository;
            this.hubCurrentOrganizationStructure = hubCurrentOrganizationStructure;
        }

        OrganizationalStructureVM GetById(long id)
        {
            Func<long, OrganizationalStructureVM> fn = (orgId) =>
            {
                return repository.Table.Where(o => o.Id == id)
                .Select(o => new OrganizationalStructureVM
                {
                    Id = o.Id,
                    Abbrev = o.Abbrev,
                    Description = o.Description,
                    IsDomain = o.IsDomain,
                    IsLeaf = o.IsLeaf,
                    IsRoot = o.IsRoot,
                    Inactive = o.Inactive,
                    Father_Id = o.Father.Id,
                    Father_Description = o.Father.Description
                }).FirstOrDefault();
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(id));
        }

        public OrganizationalStructureVM GetCurrent()
        {
            var currentId = hubCurrentOrganizationStructure.Get();

            if (string.IsNullOrEmpty(currentId)) return null;

            return GetById(long.Parse(currentId));
        }

        public OrganizationalStructureVM GetCurrentDomain(long? structId = null)
        {
            var current = GetCurrent();

            var org = structId == null ? current : current.Id == structId ? current : GetById(structId.Value);

            if (org.IsDomain)
            {
                return org;
            }
            if (org.IsRoot)
            {
                return null;
            }

            return GetById(org.Father_Id.Value);
        }

        public OrganizationalStructureVM GetCurrentRoot()
        {
            var domain = GetCurrentDomain();

            if (domain == null) return null;

            return GetById(domain.Father_Id.Value);
        }

        //public TimeZoneInfo GetCurrentTimezone()
        //{
        //    var currentId = hubCurrentOrganizationStructure.Get();

        //    if (string.IsNullOrEmpty(currentId)) return null;

        //    return GetTimezone(long.Parse(currentId));
        //}

        //public TimeZoneInfo GetTimezone(long id)
        //{
        //    Func<long, string> fn = (orgId) =>
        //    {
        //        return Engine.Resolve<IRepository<Establishment>>().Table.Where(e => e.OrganizationalStructure.Id == id).Select(e => e.Timezone).FirstOrDefault();
        //    };

        //    var timezone = Engine.Resolve<CacheManager>().CacheAction(() => fn(id));

        //    return TimeZoneInfo.FindSystemTimeZoneById(timezone);
        //}

        public OrganizationalStructureVM Set(long id)
        {
            hubCurrentOrganizationStructure.Set(id.ToString());

            return GetById(id);
        }

        public void Set(OrganizationalStructureVM org)
        {
            hubCurrentOrganizationStructure.Set(org.Id.ToString());
        }

        public void SetByCookieData(string cookieData)
        {
            if (string.IsNullOrEmpty(cookieData)) return;

            var model = JsonConvert.DeserializeObject<OrganizationalStructureVM>(cookieData);

            hubCurrentOrganizationStructure.Set(model.Id.ToString());
        }
    }
}
