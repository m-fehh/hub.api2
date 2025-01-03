using Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hub.Migrator.Seeders.Data
{
    public class OrganizationalStructureSeeder : ISeeder
    {
        public int Order => 5;


        public async Task SeedAsync(EntityDbContext dbContext)
        {
            var add = new OrganizationalStructure
            {
                Abbrev = "GRP",
                Description = "Grupo Corporativo",
                Inactive = false,
                IsRoot = true,
                IsLeaf = false,
                IsDomain = false,
                ExternalCode = "ORG-001-GRP"
            };

            var exists = await dbContext.OrganizationalStructures.AnyAsync(x => x.Abbrev.Equals(add.Abbrev));
            if (!exists)
            {
                dbContext.OrganizationalStructures.Add(add);
            }

            await dbContext.SaveChangesAsync(); 
        }
    }
}
