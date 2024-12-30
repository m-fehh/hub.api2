namespace Hub.Infrastructure.Database.Entity.Interfaces
{
    public interface IModificationControl : IBaseEntity
    {
        DateTime? CreationUTC { get; set; }
        DateTime? LastUpdateUTC { get; set; }
    }
}
