using Hub.Infrastructure.Database.Models.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Hub.Migrator.Seeders.Data
{
    public class OrganizationalStructureSeeder : ISeeder
    {
        public int Order => 5;

        public async Task SeedAsync(EntityDbContext dbContext)
        {
            var newStructure = new OrganizationalStructure
            {
                Abbrev = "GRP",
                Description = "Grupo Corporativo",
                Inactive = false,
                IsRoot = true,
                IsLeaf = false,
                IsDomain = false,
                ExternalCode = "ORG-001-GRP",
                Tree = "XXX"
            };

            if (!await dbContext.OrganizationalStructures.AnyAsync(x => x.Abbrev == newStructure.Abbrev))
            {
                dbContext.OrganizationalStructures.Add(newStructure);
                await dbContext.SaveChangesAsync();

                newStructure.Tree = GenerateTree(newStructure);
                dbContext.OrganizationalStructures.Update(newStructure);
                await dbContext.SaveChangesAsync();
            }
        }

        private static string GenerateTree(OrganizationalStructure entity)
        {
            return $"({entity.Id})";
        }
    }
}
