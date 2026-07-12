using MyHomeRamen.Domain.Orders.Roles;

namespace MyHomeRamen.Features.Orders.Features.Roles.Common;

public interface IRoleSpecification
{
    Task<Role?> ByName(string orderRoleName, CancellationToken cancellationToken);
}
