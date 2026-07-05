using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Features.Payments.Features.Roles.Common;

public interface IRoleSpecification
{
    Task<Role?> ByIdAsync(RoleId id, CancellationToken cancellationToken);
}
