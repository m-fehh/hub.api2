using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;
using Hub.Infrastructure.Architecture.Localization;

namespace Hub.Application.Services
{
    public class PortalUserSettingService : OrchestratorService<PortalUserSetting>, IUserSettingManager
    {
        public PortalUserSettingService(IRepository<PortalUserSetting> repository) : base(repository) { }

        private void Validate(PortalUserSetting entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new BusinessException(entity.DefaultRequiredMessage(e => e.Name));
            }
        }

        private void ValidadeInsert(PortalUserSetting entity)
        {
            Validate(entity);
        }

        public override long Insert(PortalUserSetting entity)
        {
            ValidadeInsert(entity);

            using (var transaction = base._repository.BeginTransaction())
            {
                var ret = base._repository.Insert(entity);

                if (transaction != null) base._repository.Commit();

                return ret;
            }
        }

        public override void Update(PortalUserSetting entity)
        {
            Validate(entity);

            using (var transaction = base._repository.BeginTransaction())
            {
                base._repository.Update(entity);

                if (transaction != null) base._repository.Commit();
            }
        }

        public override void Delete(long id)
        {
            using (var transaction = base._repository.BeginTransaction())
            {
                base._repository.Delete(id);

                if (transaction != null) base._repository.Commit();
            }
        }

        public void SaveSetting(string key, string value)
        {
            var currentUserId = Engine.Resolve<ISecurityProvider>().GetCurrentId();

            var setting = Table.FirstOrDefault(o => o.Name == key && o.PortalUser.Id == currentUserId);

            if (setting == null)
            {
                Insert(new PortalUserSetting()
                {
                    PortalUser = new PortalUser() { Id = currentUserId.Value },
                    Name = key,
                    Value = value
                });
            }
            else
            {
                setting.Value = value;

                Update(setting);
            }
        }

        public string GetSetting(string key)
        {
            var currentUserId = Engine.Resolve<ISecurityProvider>().GetCurrentId();

            var setting = Table.FirstOrDefault(o => o.Name == key && o.PortalUser.Id == currentUserId);

            if (setting == null) return null;

            return setting.Value;

        }
    }
}
