using MyHomeRamen.Domain.ShoppingCart.Roles;

namespace MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

public interface IRoleSpecification
{
    Task<Role?> ByIdAsync(RoleId roleId, CancellationToken cancellationToken);
    Task<IEnumerable<Role>> GetAllWithPermissions(CancellationToken cancellationToken);
}
