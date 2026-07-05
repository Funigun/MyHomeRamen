using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

public interface IRoleSpecification
{
    Task<Role?> ByIdAsync(RoleId roleId, CancellationToken cancellationToken);
}
