using Hub.Infrastructure.HealthChecker.Interfaces;

namespace Hub.Infrastructure.HealthChecker
{
    public class CheckerContainer : ICheckerContainer
    {
        public object Father { get; set; }
        public List<ICheckerItem> Items { get; set; } = new List<ICheckerItem>();

        public CheckerContainer()
        {
        }

        public CheckerContainer(object father)
        {
            Father = father;
        }

        public void Validate()
        {
            foreach (var item in Items)
            {
                item.Validate();
            }
        }
    }
}
