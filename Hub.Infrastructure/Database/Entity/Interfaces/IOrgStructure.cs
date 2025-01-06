namespace Hub.Infrastructure.Database.Entity.Interfaces
{
    public interface IOrgStructure : IBaseEntity
    {
        long Id { get; set; }

        string Abbrev { get; set; }

        string Description { get; set; }

        bool Inactive { get; set; }

        bool AppearInMobileApp { get; set; }

        bool IsRoot { get; set; }

        bool IsLeaf { get; set; }

        bool IsDomain { get; set; }

        string Tree { get; set; }
    }
}
