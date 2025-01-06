using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Migrator.Seeders.Data
{
    //public class PortalUserSeeder : ISeeder
    //{
    //    public int Order => 4;

    //    public async Task SeedAsync(EntityDbContext dbContext)
    //    {
    //        var add = new PortalUser
    //        {
    //            Name = "Administrador",
    //            Password = "123".EncodeSHA1(),
    //            Login = "admin",
    //            Email = "admin@hub.com.br",
    //            ProfileId = dbContext.ProfileGroups.FirstOrDefault(x => x.Administrator).Id,
    //            Inactive = false,
    //            ChangingPass = true,
    //            CreationUTC = DateTime.Now
    //        };

    //        var exists = await dbContext.PortalUsers.AnyAsync(x => x.Name.Equals(add.Name));
    //        if (!exists)
    //        {
    //            dbContext.PortalUsers.Add(add);
    //        }

    //        await dbContext.SaveChangesAsync();
    //    }
    //}
}
