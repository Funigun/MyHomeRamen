using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Roles;
using MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IRoleQuery
{
    async Task<Role?> IRoleQuery.ByIdAsync(RoleId roleId, CancellationToken cancellationToken)
        => await Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == roleId, cancellationToken);

    public async Task<Role?> GetByNameWithPermissionsAsync(string name, CancellationToken cancellationToken)
        => await Roles.AsNoTracking()
            .Include(role => role.Permissions)
            .FirstOrDefaultAsync(role => role.Name == name, cancellationToken);
}
