using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Identity.Api.Application;
using MyHomeRamen.Identity.Api.Domain;

namespace MyHomeRamen.Identity.Api.Persistance;

public class DbService(AppDbContext dbContext, RestaurantConfigurationProvider configurationProvider)
{
    public async Task SeedDatabase()
    {
        await SeedRoles();
        await SeedPermissions();
    }

    private async Task SeedRoles()
    {
        if (!await dbContext.Roles.AnyAsync())
        {
            IEnumerable<Role> roles = AuthenticationConstants.Roles.Select(role => new Role()
            {
                Name = role,
                RestaurantId = configurationProvider.RestaurantId
            });

            await dbContext.Roles.AddRangeAsync(roles);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedPermissions()
    {
        if (!await dbContext.Permissions.AnyAsync())
        {
            IEnumerable<Permission> permissions = AuthenticationConstants.Permissions.Select(permission => new Permission()
            {
                Name = permission,
                RestaurantId = configurationProvider.RestaurantId
            });

            await dbContext.Permissions.AddRangeAsync(permissions);
            await dbContext.SaveChangesAsync();
        }
    }
}
