using Hub.Infrastructure.Architecture.Autofac.Interfaces;

namespace Hub.Infrastructure.Architecture.HealthChecker.Interfaces
{
    public interface ICheckerContainer : IValidable
    {
        object Father { get; set; }
        List<ICheckerItem> Items { get; set; }
    }
}
