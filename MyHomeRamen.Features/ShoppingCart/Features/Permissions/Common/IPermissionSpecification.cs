using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;

public interface IPermissionSpecification
{
    Task<Permission?> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken);
}
