using Hub.Domain.Migrations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Hub.Domain
{
    public class MigrationsDbContextFactory : IDesignTimeDbContextFactory<EntityDbContext>
    {
        public EntityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EntityDbContext>();

            return new EntityDbContext(builder.Options, new MigrationsTenantProvider());
        }
    }
}
