using Hub.Application.Services.Users;
using Hub.Domain.Entities.Enterprise;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;
using Hub.Infrastructure.Architecture.Localization;
using Hub.Infrastructure.Database.Models.Tenant;
using Hub.Application.Corporate.Interfaces;
using Hub.Application.Corporate.Manager;
using Hub.Infrastructure.Database.Models.Helpers;
using Hub.Infrastructure.Extensions;

namespace Hub.Application.Services.Enterprise
{
    public class OrganizationalStructureService : OrchestratorService<OrganizationalStructure>, IOrganizationalStructureService
    {
        private static object locker = new object();
        private static long? currentOrgStructureIfNull;

        public OrganizationalStructureService(IRepository<OrganizationalStructure> repository) : base(repository) { }

        public override long Insert(OrganizationalStructure entity)
        {
            ValidateInsert(entity);

            SetRootProperty(entity);

            entity.Tree = GenerateTree(entity);

            using (var transaction = _repository.BeginTransaction())
            {
                var ret = _repository.Insert(entity);

                if (transaction != null) _repository.Commit();

                using (var nestedTransaction = _repository.BeginTransaction())
                {
                    var currentUser = (PortalUser)Engine.Resolve<ISecurityProvider>().GetCurrent();

                    if (currentUser != null)
                    {
                        currentUser.OrganizationalStructures.Add(entity);

                        Engine.Resolve<IRedisService>().Set($"UserOrgList{currentUser.Id}", null);

                        Engine.Resolve<IRepository<PortalUser>>().Update(currentUser);
                    }

                    entity.Tree = GenerateTree(entity);

                    _repository.Update(entity);

                    if (nestedTransaction != null) _repository.Commit();
                }

                return ret;
            }
        }

        public override void Update(OrganizationalStructure entity)
        {
            Validate(entity);

            SetRootProperty(entity);

            entity.Tree = GenerateTree(entity);

            using (var transaction = _repository.BeginTransaction())
            {

                _repository.Update(entity);

                if (transaction != null) _repository.Commit();
            }
        }

        public void UpdateLastUpdateUTC(long orgStructureId)
        {
            var entity = GetById(orgStructureId);

            entity.LastUpdateUTC = DateTime.UtcNow;

            this.Update(entity);
        }

        public override void Delete(long id)
        {
            using (var transaction = _repository.BeginTransaction())
            {
                var entity = GetById(id);

                _repository.Delete(entity);

                if (transaction != null) _repository.Commit();
            }
        }

        public bool IsLeafStructure(long? structId = null)
        {
            if (structId == null)
            {
                structId = Engine.Resolve<IHubCurrentOrganizationStructure>().Get().ToLong();
            }

            Func<long, bool> fn = (s) =>
            {
                return _repository.Table.Where(o => o.Id == s).Select(o => o.IsLeaf).First();
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(structId.Value), localCacheTimeSeconds: 60, redisCacheTimeSeconds: 600);
        }

        public bool IsDomainStructure(long? structId = null)
        {
            if (structId == null)
            {
                structId = Engine.Resolve<IHubCurrentOrganizationStructure>().Get().ToLong();
            }


            Func<long, bool> fn = (s) =>
            {
                return _repository.Table.Where(o => o.Id == s).Select(o => o.IsDomain).First();
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(structId.Value), localCacheTimeSeconds: 60, redisCacheTimeSeconds: 600);
        }

        public bool IsRootStructure(long? structId = null)
        {
            if (structId == null)
            {
                structId = Engine.Resolve<IHubCurrentOrganizationStructure>().Get().ToLong();
            }

            Func<long, bool> fn = (s) =>
            {
                return _repository.Table.Where(o => o.Id == s).Select(o => o.IsRoot).First();
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(structId.Value), localCacheTimeSeconds: 60, redisCacheTimeSeconds: 600);
        }

        public string GetConfigByName(OrganizationalStructure org, string name)
        {
            return GetConfigByName(org != null ? org.Id : null, name);
        }

        public string GetConfigByName(long? orgId_, string name_)
        {
            Func<long?, string, string> fn = (orgId, name) =>
            {
                if (orgId == null)
                {
                    return Engine.Resolve<IRepository<OrgStructConfigDefault>>().Table.Where(c => c.Name == name).Select(c => c.DefaultValue).FirstOrDefault();
                }

                var configValue = Engine.Resolve<IRepository<OrganizationalStructureConfig>>().Table.Where(o => o.OrganizationalStructure.Id == orgId && o.Config.Name == name).Select(o => o.Value).FirstOrDefault();

                if (string.IsNullOrEmpty(configValue))
                {
                    configValue = Engine.Resolve<IRepository<OrgStructConfigDefault>>().Table.Where(c => c.Name == name).Select(c => c.DefaultValue).FirstOrDefault();
                }

                return configValue;
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(orgId_, name_), key: "OrganizationalStructureConfigs", redisCacheTimeSeconds: 300);
        }

        public void SetConfig(OrganizationalStructure org, string name, string value)
        {
            var redisService = Engine.Resolve<IRedisService>();

            redisService.DeleteFromPattern($"CacheManager:OrganizationalStructureConfigs*{org.Id}_{name}*");
        }

        public string GetCurrentConfigByName(string name)
        {
            var currentLevel = Engine.Resolve<IHubCurrentOrganizationStructure>().Get();

            return GetConfigByName(currentLevel.ToLong(), name);
        }

        public OrganizationalStructure GetFather(long childId)
        {
            var father = _repository.Table.Where(w => w.Id == childId).Select(s => s.Father).FirstOrDefault();
            return father;
        }

        public string GetConfigByNameFromRoot(string name)
        {
            var service = Engine.Resolve<IHubCurrentOrganizationStructure>();
            var root = service.GetCurrentRoot();
            var configName = GetConfigByName(root, name);
            return !string.IsNullOrWhiteSpace(configName) && configName != "-" ? configName : "";
        }

        public Establishment GetCurrentEstablishment(string currentStringLevel = null)
        {
            if (string.IsNullOrEmpty(currentStringLevel))
            {
                currentStringLevel = Engine.Resolve<IHubCurrentOrganizationStructure>().Get();
            }

            if (string.IsNullOrEmpty(currentStringLevel)) return null;

            var currentLevel = long.Parse(currentStringLevel);

            return Engine.Resolve<IRepository<Establishment>>().Table.Where(r => r.OrganizationalStructure.Id == currentLevel).FirstOrDefault();
        }

        public TimeZoneInfo GetCurrentEstablishmentTimeZone(string currentStringLevel = null)
        {
            Func<string, TimeZoneInfo> fn = (orgLevel) =>
            {
                var redisService = Engine.Resolve<IRedisService>();

                var cachedTimeZone = redisService.Get($"TimeZone{orgLevel}").ToString();

                if (!string.IsNullOrEmpty(cachedTimeZone))
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(cachedTimeZone);
                }

                var establishemnt = Engine.Resolve<OrganizationalStructureService>().GetCurrentEstablishment(orgLevel);

                if (establishemnt != null && !string.IsNullOrEmpty(establishemnt.TimezoneIdentifier))
                {
                    redisService.Set($"TimeZone{orgLevel}", establishemnt.TimezoneIdentifier);

                    return TimeZoneInfo.FindSystemTimeZoneById(establishemnt.TimezoneIdentifier);
                }

                return null;
            };

            if (string.IsNullOrEmpty(currentStringLevel))
            {
                var localcached = Engine.Resolve<PortalCacheManager>().Get().CurrentTimezone;

                if (!string.IsNullOrEmpty(localcached))
                {
                    if (localcached == "-") return null;

                    return TimeZoneInfo.FindSystemTimeZoneById(localcached);
                }

                currentStringLevel = Engine.Resolve<IHubCurrentOrganizationStructure>().Get();
            }

            if (string.IsNullOrEmpty(currentStringLevel)) return null;

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(currentStringLevel));
        }

        //public void ChangeOwnerOrgStruct(string objectType, long objectId, long newOwnerOrgStructId)
        //{
        //    using (var transaction = base._repository.BeginTransaction())
        //    {
        //        var type = Type.GetType($"SCH.Core.Entity.{objectType}");

        //        var hasModifitionControls = type?.GetInterfaces().Contains(typeof(IModificationControl));

        //        var newOwnerOrgStruct = GetById(newOwnerOrgStructId);

        //        if (hasModifitionControls ?? false)
        //        {
        //            _repository.CreateQuery($"update {objectType} set OwnerOrgStruct = :newOrg, LastUpdateUTC = :lasUtc where Id = :objectId")
        //                .SetParameter("newOrg", newOwnerOrgStruct)
        //                .SetParameter("objectId", objectId)
        //                .SetParameter("lasUtc", DateTime.UtcNow)
        //                .ExecuteUpdate();

        //        }
        //        else
        //        {
        //            _repository.CreateQuery($"update {objectType} set OwnerOrgStruct = :newOrg where Id = :objectId")
        //                .SetParameter("newOrg", newOwnerOrgStruct)
        //                .SetParameter("objectId", objectId)
        //                .ExecuteUpdate();

        //        }

        //        if (transaction != null) base._repository.Commit();

        //        if (type != null)
        //        {
        //            var notificationType = typeof(IUpdateEntityNotification<>).MakeGenericType(type);

        //            object notificationObject;

        //            if (Engine.TryResolve(notificationType, out notificationObject))
        //            {
        //                var idProd = notificationType.GetProperty("Id");
        //                var actionProd = notificationType.GetProperty("Action");

        //                idProd.SetValue(notificationObject, objectId);
        //                actionProd.SetValue(notificationObject, ELogAction.Update);

        //                Engine.Resolve<IMediator>().Publish((INotification)notificationObject);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Verifica se a configuração para uso do apelido está ativada para o domínio da unidade passada
        /// </summary>
        /// <param name="structureId">unidade ou domínio em que se dejesa verificar a configuração</param>
        /// <returns><b>False</b> caso o parametro seja o dominio raiz, caso contrário, busca no banco de dados o valor da configuração da unidade.</returns>
        public bool ClientUseNickName(long structureId)
        {
            if (structureId == 0)
            {
                return false;
            }

            var isCurrentOrgStructRoot = _repository.Table.Where(f => f.Id == structureId).Select(s => s.IsRoot).FirstOrDefault();

            if (isCurrentOrgStructRoot)
            {
                return false;
            }

            var organizationStructure = Engine.Resolve<IHubCurrentOrganizationStructure>();

            var currentDomain = long.Parse(organizationStructure.GetCurrentDomain(structureId.ToString()));

            return bool.Parse(GetConfigByName(currentDomain, "NicknameVisible"));
        }

        /// <summary>
        /// Método responsável por gerar a árvore de estabelecimentos 
        /// </summary>
        /// <param name="removeNotAccessOrg"></param>
        /// <returns></returns>
        //public List<OrganizationalStructureTreeModelVM> GenerateTreeList(bool removeNotAccessOrg = true)
        //{

        //    var allItens = (from os in _repository.Table
        //                    where os.Inactive == false && os.Father != null
        //                    select new OrganizationalStructureVM
        //                    {
        //                        Id = os.Id,
        //                        Description = os.Description,
        //                        Father_Id = os.Father.Id,
        //                        Inactive = os.Inactive
        //                    }).ToList();

        //    var rootList = new List<OrganizationalStructureTreeModelVM>();

        //    rootList = (from r in _repository.Table
        //                where r.Father == null
        //                select new OrganizationalStructureTreeModelVM { Id = r.Id, Description = r.Description }).ToList();


        //    GenerateSubTreeList(rootList, allItens.ToList());

        //    return rootList;
        //}

        public bool AllowChanges<TEntity>(TEntity entity, bool thowsException = true) where TEntity : IBaseEntity, IEntityOrgStructOwned
        {
            var repository = Engine.Resolve<IRepository<TEntity>>();

            var currentOrgStruct = repository.Table.Where(e => e.Id == entity.Id).Select(e => e.OwnerOrgStruct).FirstOrDefault();

            var currentuserOrgIds = Engine.Resolve<UserService>().GetCurrentUserOrgList();

            if (currentuserOrgIds != null)
            {
                if (currentOrgStruct != null && !currentuserOrgIds.Contains(currentOrgStruct.Id))
                {
                    if (thowsException)
                    {
#if DEBUG
                        return false;
#else
                        throw new BusinessException(Engine.Get("NotAllowedChangedBecauseOwnerOrgStruct"));
#endif
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            entity.OwnerOrgStruct = currentOrgStruct;

            return true;
        }

        public void LinkOwnerOrgStruct(IEntityOrgStructOwned entity)
        {
            var currentLevel = Engine.Resolve<IHubCurrentOrganizationStructure>().Get();

            if (string.IsNullOrEmpty(currentLevel))
            {
                if (currentOrgStructureIfNull == null)
                {
                    throw new BusinessException(Engine.Get("SelectOrgStruct"));
                }

                currentLevel = currentOrgStructureIfNull.ToString();
            }

            var orgStruct = GetById(long.Parse(currentLevel));

            if (orgStruct == null || orgStruct.Inactive)
            {
                throw new BusinessException(Engine.Get("SelectOrgStruct"));
            }

            entity.OwnerOrgStruct = orgStruct;
        }


        #region PRIVATE METHODS

        //private void GenerateSubTreeList(List<OrganizationalStructureTreeModelVM> rootList, List<OrganizationalStructureVM> allItens)
        //{
        //    foreach (var item in rootList)
        //    {
        //        item.Items =
        //            allItens.Where(i =>
        //                i.Father_Id != null &&
        //                i.Father_Id == item.Id)
        //            .Select(i => new OrganizationalStructureTreeModelVM()
        //            {
        //                Id = i.Id.Value,
        //                Description = i.Description,
        //                FatherId = i.Father_Id,
        //                Inactive = i.Inactive

        //            }).ToList();

        //        GenerateSubTreeList(item.Items, allItens);
        //    }
        //}

        private string GenerateTree(OrganizationalStructure entity)
        {
            string returnList = "(" + entity.Id.ToString() + ")";

            if (entity.Father != null)
            {
                var parent = _repository.Table.FirstOrDefault(p => p.Id == entity.Father.Id);

                returnList = GenerateTree(parent) + "," + returnList;
            }

            return returnList;
        }

        private void SetRootProperty(OrganizationalStructure entity)
        {
            if (entity.Father == null)
                entity.IsRoot = true;
            else
                entity.IsRoot = false;
        }

        private void Validate(OrganizationalStructure entity)
        {
            if (Table.Any(u => u.Description == entity.Description && u.Id != entity.Id))
            {
                throw new BusinessException(entity.DefaultAlreadyRegisteredMessage(e => e.Description));
            }

            if (Table.Any(u => u.Abbrev == entity.Abbrev && u.Id != entity.Id))
            {
                throw new BusinessException(entity.DefaultAlreadyRegisteredMessage(e => e.Abbrev));
            }

            if (entity.IsLeaf && Table.Any(u => u.Father.Id == entity.Id))
            {
                throw new BusinessException(Engine.Get("OrgStructCantBeLast"));
            }


            ValidateIsParent(entity, entity.Father);
        }

        private void ValidateIsParent(OrganizationalStructure father, OrganizationalStructure children)
        {
            if (father == null || children == null) return;

            children = GetById(children.Id);

            if (children.Father == null) return;

            if (children.Father == father)
            {
                throw new BusinessException(Engine.Get("OrgStructCircularRef"));
            }

            ValidateIsParent(father, GetById(children.Father.Id));
        }

        private void ValidateDomainTree(OrganizationalStructure entity)
        {
            this.ValidateDomainAncestors(entity);
            this.ValidateDomainDescendants(entity);
        }

        private void ValidateDomainDescendants(OrganizationalStructure entity)
        {
            if (entity.Childrens != null && entity.Childrens.Count > 0)
            {
                if (entity.Childrens.Any(c => c.IsDomain))
                    throw new BusinessException(Engine.Get("OrgStructAlreadyHaveDescendantDomain"));
                else
                {
                    foreach (OrganizationalStructure child in entity.Childrens)
                    {
                        ValidateDomainDescendants(child);
                    }
                }
            }
        }

        private void ValidateDomainAncestors(OrganizationalStructure entity)
        {
            if (entity.Father != null)
            {
                var father = GetById(entity.Father.Id);

                if (father.IsDomain)
                    throw new BusinessException(Engine.Get("OrgStructAlreadyHaveAncestorDomain"));
                else
                    ValidateDomainAncestors(father);
            }
        }

        private void ValidateInsert(OrganizationalStructure entity)
        {
            Validate(entity);
        }

        #endregion
    }
}
