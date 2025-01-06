using Hub.Infrastructure.Architecture.Autofac.Interfaces;

namespace Hub.Infrastructure.Architecture.HealthChecker.Interfaces
{
    public interface ICheckerItem : IValidable
    {
        ICheckerContainer Container { get; set; }
        string ErrorMessage { get; }
    }

    public interface ICheckerItem<T> : ICheckerItem
    {
        Func<T> Func { get; }
        Func<T, bool> IsHealthy { get; }
        Func<bool> Condition { get; }
    }
}
