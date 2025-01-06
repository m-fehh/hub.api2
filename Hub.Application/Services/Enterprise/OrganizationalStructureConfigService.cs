using Hub.Domain.Entities.Enterprise;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services.Enterprise
{
    public class OrganizationalStructureConfigService : OrchestratorService<OrganizationalStructureConfig>
    {
        private readonly CacheManager _cacheManager;

        public OrganizationalStructureConfigService(IRepository<OrganizationalStructureConfig> repository, CacheManager cacheManager) : base(repository) 
        {
            this._cacheManager = cacheManager;

        }

        public override long Insert(OrganizationalStructureConfig entity)
        {
            Validate(entity);

            AdjustConfigValue(entity);

            using (var transaction = _repository.BeginTransaction())
            {
                var ret = _repository.Insert(entity);

                var config = FetchDefaultConfig(entity.Config);

                ClearCache(entity.OrganizationalStructure.Id, config);

                Engine.Resolve<OrganizationalStructureService>().SetConfig(entity.OrganizationalStructure, config.Name, entity.Value);

                if (transaction != null) _repository.Commit();

                return ret;
            }
        }

        public override void Update(OrganizationalStructureConfig entity)
        {
            AdjustConfigValue(entity);

            Validate(entity);

            /* TODO
             
                        var schema = "sch" + Engine.Resolve<ITenantManager>().GetInfo().Id;
            var oldConfigValue = Engine.Resolve<IRepository<Establishment>>().CreateSQLQuery(String.Format("Select Value from {0}.OrganizationalStructureConfig Where Id = {1}", schema, entity.Id))
                .SetResultTransformer(Transformers.AliasToBean(typeof(OrganizationalStructureConfigDTO))).List<OrganizationalStructureConfigDTO>().FirstOrDefault();
             
             */

            using (var transaction = base._repository.BeginTransaction())
            {
                //LogConfigurationChange(oldConfigValue.Value, entity);

                _repository.Update(entity);

                var config = FetchDefaultConfig(entity.Config);

                ClearCache(entity.OrganizationalStructure.Id, config);

                Engine.Resolve<OrganizationalStructureService>().SetConfig(entity.OrganizationalStructure, config.Name, entity.Value);

                if (transaction != null) base._repository.Commit();
            }
        }

        public override void Delete(long id)
        {
            using (var transaction = base._repository.BeginTransaction())
            {
                var entity = GetById(id);

                var config = FetchDefaultConfig(entity.Config);

                ClearCache(entity.OrganizationalStructure.Id, config);

                Engine.Resolve<OrganizationalStructureService>().SetConfig(entity.OrganizationalStructure, config.Name, config.DefaultValue);

                base._repository.Delete(id);

                if (transaction != null) base._repository.Commit();
            }
        }

        public void AdjustConfigValue(OrganizationalStructureConfig entity)
        {
            if (string.IsNullOrEmpty(entity.Value))
            {
                entity.Value = "-";

                if (entity.Config.ConfigType != null)
                {
                    if (entity.Config.ConfigType.Equals("Boolean") || entity.Config.ConfigType.Equals("Double") || entity.Config.ConfigType.Equals("Int32"))
                    {
                        entity.Value = "0";
                    }
                }
            }
        }

        public bool LogConfigurationChange(string previousValue, OrganizationalStructureConfig model)
        {
            var configChanged = string.Compare(previousValue, model.Value, true) != 0;

            if (configChanged)
            {
                string value;

                if (string.Compare(model.Value, "True", true) == 0 || string.Compare(model.Value, "False", true) == 0)
                {
                    value = string.Compare(model.Value, "True", true) == 0 ? Engine.Get("Yes") : Engine.Get("No");

                }
                else
                {
                    value = model.Value;
                }

                string message = string.Format(Engine.Get("ConfigChangedMessage"), model.OrganizationalStructure.Description, model.Config.Name);

                Engine.Resolve<LogService>().Audit(Engine.Get(""), model.Id, ELogAction.Update, 0, "", "", message);
            }

            return configChanged;
        }


        #region PRIVATE METHODS

        private void Validate(OrganizationalStructureConfig entity)
        {
            var config = (OrgStructConfigDefault)_repository.Refresh(entity.Config);

            if (config.ConfigType == "Int32")
            {
                if (string.IsNullOrEmpty(entity.Value))
                {
                    throw new BusinessException(string.Format(Engine.Get("DefaultRequiredMessage"), Engine.Get(config.Name)));
                }

                long v;

                if (!Int64.TryParse(entity.Value, out v))
                {
                    throw new BusinessException(string.Format(Engine.Get("generic_invalid_message"), Engine.Get(config.Name)));
                }
            }

            if (config.ConfigType == "Double")
            {
                if (string.IsNullOrEmpty(entity.Value))
                {
                    throw new BusinessException(string.Format(Engine.Get("DefaultRequiredMessage"), Engine.Get(config.Name)));
                }

                Double v;

                if (!Double.TryParse(entity.Value, out v))
                {
                    throw new BusinessException(string.Format(Engine.Get("generic_invalid_message"), Engine.Get(config.Name)));
                }
            }
        }

        private OrgStructConfigDefault FetchDefaultConfig(OrgStructConfigDefault model)
        {
            return (OrgStructConfigDefault)_repository.Refresh(model);
        }

        private void ClearCache()
        {
            _cacheManager.InvalidateCacheAction("OrganizationalStructureConfigs");

            _cacheManager.InvalidateCacheAction("EstablishmentCacheConfig");
        }

        private void ClearCache(long organizatinalStructureId, OrgStructConfigDefault config)
        {
            _cacheManager.InvalidateCacheAction("OrganizationalStructureConfigs");

            string establishmentCacheKey = $"EstablishmentCacheConfig_{config.Name}";

            var entity = Engine.Resolve<OrganizationalStructureService>().GetById(organizatinalStructureId);

            if (entity != null && !entity.IsRoot)
            {
                establishmentCacheKey += $"_{entity.Id}";
            }

            _cacheManager.InvalidateCacheAction(establishmentCacheKey);
        }

        //private void SaveConfigs(List<OrganizationalStructureConfigPreserved> configs)
        //{
        //    foreach (var config in configs)
        //    {
        //        var exists = _organizationalStructureConfigPreservedRepository.Table.FirstOrDefault(f =>
        //        f.AttachedToValueId == config.AttachedToValueId &&
        //        f.OrganizationalStructure.Id == config.OrganizationalStructure.Id &&
        //        f.Config.Id == config.Config.Id);

        //        using (var transaction = _organizationalStructureConfigPreservedRepository.BeginTransaction())
        //        {
        //            if (exists == null)
        //            {
        //                _organizationalStructureConfigPreservedRepository.Insert(config);
        //            }
        //            else
        //            {
        //                exists.Value = config.Value;
        //                exists.LastUpdateUTC = DateTime.UtcNow;

        //                _organizationalStructureConfigPreservedRepository.Update(exists);
        //            }

        //            if (transaction != null) _organizationalStructureConfigPreservedRepository.Commit();
        //        }
        //    }
        //}

        #endregion
    }
}
