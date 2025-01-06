namespace Hub.Infrastructure.Database.Entity.Interfaces
{
    public interface IUserAccount : IBaseEntity
    {
        string Login { get; set; }

        string Name { get; set; }

        public string Email { get; set; }

        public string IpAddress { get; set; }

        IProfileGroup Profile { get; set; }
    }
}
