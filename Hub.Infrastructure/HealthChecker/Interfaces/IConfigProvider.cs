namespace Hub.Infrastructure.HealthChecker.Interfaces
{
    public interface IConfigProvider
    {
        string Get(string configuration);
    }
}
