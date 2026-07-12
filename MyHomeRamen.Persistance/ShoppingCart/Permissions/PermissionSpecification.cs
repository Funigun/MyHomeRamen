using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Permissions;
using MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IPermissionSpecification
{
    public async Task<Permission?> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken)
        => await Permissions.FirstOrDefaultAsync(permission => permission.Id == permissionId, cancellationToken);
}
