﻿using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;

namespace Hub.Application.CorporateStructure.Interfaces
{
    public interface IOrgStructBasedService
    {
        void LinkOwnerOrgStruct(IEntityOrgStructOwned entity);

        bool AllowChanges<TEntity>(TEntity entity, bool thowsException = true) where TEntity : IBaseEntity, IEntityOrgStructOwned;

        OrganizationalStructure GetFather(long childId);
        string GetConfigByNameFromRoot(string name);
    }
}
