namespace Hub.Infrastructure.Database.Entity.Interfaces
{
    public interface IProfileGroup : IBaseEntity
    {
        string Name { get; set; }

        ICollection<IAccessRule> Rules { get; set; }

        bool Administrator { get; set; }
    }
}
