using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

public interface IRoleQuery
{
    Task<Role?> ByIdAsync(RoleId roleId, CancellationToken cancellationToken);

    Task<Role?> GetByNameWithPermissionsAsync(string name, CancellationToken cancellationToken);
}
