using MyHomeRamen.Domain.Reservations.Roles;

namespace MyHomeRamen.Features.Reservations.Features.Roles.Common;

public interface IRoleQuery
{
    Task<Role?> GetByNameWithPermissionsAsync(string roleName, CancellationToken cancellationToken);
}
