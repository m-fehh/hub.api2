using Hub.Infrastructure.Database.Entity.Interfaces;

namespace Hub.Infrastructure.Architecture.Mapper.Interfaces
{
    public interface IDataMapper<TEntity, TModel> where TEntity : IBaseEntity where TModel : IModelEntity
    {
        TModel BuildModel(TEntity entity);
        TEntity BuildEntity(TModel model, long? id = null);
    }
}
