namespace Hub.Migrator.Seeders
{
    public interface ISeeder
    {
        Task SeedAsync(EntityDbContext dbContext);
    }
}
