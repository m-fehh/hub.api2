using System.Reflection;

namespace Hub.Migrator.Seeders
{
    public class SeederExecutor
    {
        private readonly EntityDbContext _dbContext;

        public SeederExecutor(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RunSeedersAsync()
        {
            // Obtém todas as classes do assembly atual
            var seederTypes = Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .Where(t => typeof(ISeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var seederType in seederTypes)
            {
                var seederInstance = (ISeeder)Activator.CreateInstance(seederType);
                await seederInstance.SeedAsync(_dbContext);
            }
        }
    }
}
