using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.TenantOrchestration.Persistency
{
    public class MigrationsDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
    {
        public SampleDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SampleDbContext>();

            return new SampleDbContext(builder.Options, new MigrationsTenantProvider());
        }
    }
}
