using MyHomeRamen.Domain.ShoppingCart.Permissions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;

public interface IPermissionQuery
{
    Task<Permission?> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken);
}
