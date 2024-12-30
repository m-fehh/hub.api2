using Hub.Infrastructure.Autofac.Interfaces;

namespace Hub.Infrastructure.HealthChecker.Interfaces
{
    public interface ICheckerContainer : IValidable
    {
        object Father { get; set; }
        List<ICheckerItem> Items { get; set; }
    }
}
