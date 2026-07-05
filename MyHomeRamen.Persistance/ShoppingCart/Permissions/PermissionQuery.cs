using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IPermissionQuery
{
    async Task<Permission?> IPermissionQuery.ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken)
        => await Permissions.AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == permissionId, cancellationToken);
}
