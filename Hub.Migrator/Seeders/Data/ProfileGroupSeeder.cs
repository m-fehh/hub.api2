using Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hub.Migrator.Seeders.Data
{
    public class ProfileGroupSeeder : ISeeder
    {
        public int Order => 1;

        public async Task SeedAsync(EntityDbContext dbContext)
        {
            var add = new ProfileGroup
            {
                Name = "Administrador",
                Administrator = true
            };

            var exists = await dbContext.ProfileGroups.AnyAsync(x => x.Name.Equals(add.Name));
            if (!exists) 
            { 
                dbContext.ProfileGroups.Add(add);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
