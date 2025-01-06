using Hub.Infrastructure.Architecture.HealthChecker;

namespace Hub.Infrastructure.Architecture.HealthChecker.Interfaces
{
    public interface IHealthChecker
    {
        CheckerContainer CheckerContainer { get; }
    }
}
