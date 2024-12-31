using Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hub.Migrator.Seeders.Data
{
    public class ProfileGroupSeeder : ISeeder
    {
        public async Task SeedAsync(EntityDbContext dbContext)
        {
            var profileAdmin = new ProfileGroup
            {
                Name = "Administrador",
                Administrator = true
            };

            var exists = await dbContext.ProfileGroups.AnyAsync(x => x.Name.Equals(profileAdmin.Name));
            if (!exists) 
            { 
                dbContext.ProfileGroups.Add(profileAdmin);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
