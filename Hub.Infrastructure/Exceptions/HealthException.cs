using Hub.Infrastructure.HealthChecker.Interfaces;

namespace Hub.Infrastructure.Exceptions
{
    public class HealthException : Exception
    {
        public ICheckerItem CheckerItem { get; private set; }

        public HealthException(ICheckerItem checkerItem) : base($"{checkerItem.ErrorMessage} - {checkerItem.Container?.Father?.GetType().FullName}")
        {
            CheckerItem = checkerItem;
        }
    }
}
