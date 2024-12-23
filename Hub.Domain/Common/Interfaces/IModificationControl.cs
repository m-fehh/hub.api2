namespace Hub.Domain.Common.Interfaces
{
    public interface IModificationControl : IBaseEntity
    {
        DateTime? CreationUTC { get; set; }
        DateTime? LastUpdateUTC { get; set; }
    }
}
