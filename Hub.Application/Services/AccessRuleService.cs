using Hub.Domain.Entities;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services
{
    public class AccessRuleService : OrchestratorService<AccessRule>
    {
        public AccessRuleService(IRepository<AccessRule> repository) : base(repository) { }

        public override long Insert(AccessRule entity)
        {
            using (var transaction = base._repository.BeginTransaction())
            {
                var ret = base._repository.Insert(entity);

                entity.Tree = GenerateTree(entity);

                base._repository.Update(entity);

                if (transaction != null) base._repository.Commit();

                return ret;
            }
        }

        public override void Update(AccessRule entity)
        {
            entity.Tree = GenerateTree(entity);

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
                var entity = base.GetById(id);

                base._repository.Delete(entity);

                if (transaction != null) base._repository.Commit();
            }
        }

        public string GenerateTree(IAccessRule entity)
        {
            if (entity == null) return "";

            string returnList = "(" + entity.Id.ToString() + ")";

            if (entity.Parent != null)
            {
                var parent = GetById(entity.Parent.Id);

                returnList = GenerateTree(parent) + "," + returnList;
            }

            return returnList;
        }
    }
}
