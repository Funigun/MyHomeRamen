using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Permissions;
using MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class PermissionRepository : IPermissionQuery
{
    async Task<Permission?> IPermissionQuery.ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == permissionId, cancellationToken);
}
