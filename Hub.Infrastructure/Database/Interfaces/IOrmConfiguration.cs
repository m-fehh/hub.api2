namespace Hub.Infrastructure.Database.Interfaces
{
    public interface IOrmConfiguration
    {
        void Configure(ConnectionStringBaseVM config);
    }
}
