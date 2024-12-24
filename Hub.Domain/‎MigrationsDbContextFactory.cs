using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Hub.Infrastructure.MultiTenant;

namespace Hub.Domain
{
    public class MigrationsDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();

            return new DatabaseContext(builder.Options, new MigrationsTenantProvider());
        }
    }
}
