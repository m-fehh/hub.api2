namespace Hub.Infrastructure.Architecture.Mapper.Interfaces
{
    public interface IModelEntity
    {
        long? Id { get; set; }

        string SerializedOldValue { get; set; }
    }
}
