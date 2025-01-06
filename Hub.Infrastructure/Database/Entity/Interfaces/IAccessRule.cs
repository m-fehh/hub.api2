namespace Hub.Infrastructure.Database.Entity.Interfaces
{
    public interface IAccessRule : IBaseEntity
    {
        IAccessRule Parent { get; set; }  
        string Description { get; set; }
        string KeyName { get; set; }
    }
}
