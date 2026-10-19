using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Identity.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Identity;

public partial class RoleRepository : IRoleLoader
{
    public Task<Role?> ByName(string roleName, CancellationToken cancellationToken, Guid? restaurantId = null)
        => identityDbContext.Roles.Include(r => r.RolePermissions)
                                  .FirstOrDefaultAsync(role => role.Name.ToLower() == roleName.ToLower()
                                                               && role.RestaurantId == restaurantId, cancellationToken);
}
