using MyHomeRamen.Domain.Payments.Roles;

namespace MyHomeRamen.Features.Payments.Features.Roles.Common;

public interface IRoleSpecification
{
    Task<Role?> ByIdAsync(RoleId id, CancellationToken cancellationToken);
}
