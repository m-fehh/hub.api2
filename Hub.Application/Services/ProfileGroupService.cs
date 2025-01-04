using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture.Localization;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Newtonsoft.Json;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Web.Services;

namespace Hub.Application.Services
{
    public class ProfileGroupService : OrchestratorService<ProfileGroup>
    {
        private readonly ISecurityProvider _securityProvider;

        public ProfileGroupService(IRepository<ProfileGroup> repository, ISecurityProvider securityProvider) : base(repository) 
        {
            this._securityProvider = securityProvider;
        }

        public void Validate(ProfileGroup entity)
        {
            if (Table.Any(u => u.Name == entity.Name && u.Id != entity.Id))
            {
                throw new BusinessException(entity.DefaultAlreadyRegisteredMessage(e => e.Name));
            }
        }

        public void ValidadeInsert(ProfileGroup entity)
        {
            //if (!Boolean.Parse(Engine.Resolve<OrganizationalStructureService>().GetCurrentConfigByName("AllowRegisterProfileGroup")))
            //{
            //    throw new BusinessException(Engine.Get("NotAllowedChangedBecauseOrgStructConfig"));
            //}

            Validate(entity);
        }

        public override long Insert(ProfileGroup entity)
        {
            ValidadeInsert(entity);


            using (var transaction = _repository.BeginTransaction())
            {
                var ret = _repository.Insert(entity);

                if (transaction != null) _repository.Commit();

                _repository.Refresh(entity);

                SaveAppProfileRoles(entity);

                return ret;
            }
        }

        public override void Update(ProfileGroup entity) { }

        public override void Delete(long id)
        {
            if (Engine.Resolve<IRepository<PortalUser>>().Table.Any(p => p.Profile.Id == id))
            {
                throw new BusinessException(Engine.Get("CantDeletePortalUserReference"));
            }

            using (var transaction = _repository.BeginTransaction())
            {
                var entity = GetById(id);

                _repository.Delete(id);

                if (transaction != null) _repository.Commit();

                DeleteAppProfileRoles(id);
            }
        }

        public IEnumerable<string> GetAppProfileRoles(long id)
        {
            var cacheKey = $"ProfileRoles{id}";
            var redisService = Engine.Resolve<IRedisService>();

            var profileRoles = redisService.Get(cacheKey).ToString();

            if (!string.IsNullOrEmpty(profileRoles))
            {
                return JsonConvert.DeserializeObject<IEnumerable<string>>(profileRoles);
            }

            ProfileGroup profile = Table.Where(p => p.Id == id).FirstOrDefault();

            if (profile == null)
            {
                return new List<string>();
            }

            return SaveAppProfileRoles(profile);
        }

        #region PRIVATE METHODS 

        private List<string> SaveAppProfileRoles(ProfileGroup entity)
        {
            var redisService = Engine.Resolve<IRedisService>();
            var cacheKey = $"ProfileRoles{entity.Id}";
            var value = new List<string>();

            if (entity.Administrator)
            {
                value.Add("ADMIN");
            }
            else
            {
                value.AddRange(Table.Where(p => p.Id == entity.Id).SelectMany(p => p.Rules).Select(r => r.KeyName).ToList());
            }

            redisService.Set(cacheKey, JsonConvert.SerializeObject(value));
            return value;
        }

        private void DeleteAppProfileRoles(long id)
        {
            var cacheKey = $"ProfileRoles{id}";
            var redisService = Engine.Resolve<IRedisService>();
            redisService.Delete(cacheKey);
        } 

        #endregion
    }
}
