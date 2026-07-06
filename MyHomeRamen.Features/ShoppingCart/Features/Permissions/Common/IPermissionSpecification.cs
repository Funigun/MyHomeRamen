using MyHomeRamen.Domain.ShoppingCart.Permissions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;

public interface IPermissionSpecification
{
    Task<Permission?> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken);
}
