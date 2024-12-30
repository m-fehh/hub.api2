namespace Hub.Infrastructure.HealthChecker.Interfaces
{
    public interface IHealthChecker
    {
        CheckerContainer CheckerContainer { get; }
    }
}
