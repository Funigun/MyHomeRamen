using MyHomeRamen.Domain.Reservations.Roles;

namespace MyHomeRamen.Features.Reservations.Features.Roles.Common;

public interface IRoleSpecification
{
    Task<Role> ByIdAsync(RoleId roleId, CancellationToken cancellationToken);
}
