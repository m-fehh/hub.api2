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
            var seederTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(ISeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).Select(t => (ISeeder)Activator.CreateInstance(t)) .OrderBy(seeder => seeder.Order).ToList();

            foreach (var seeder in seederTypes)
            {
                await seeder.SeedAsync(_dbContext);
            }
        }
    }
}
