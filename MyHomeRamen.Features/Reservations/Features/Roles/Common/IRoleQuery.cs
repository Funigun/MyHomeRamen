using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.Features.Reservations.Features.Roles.Common;

public interface IRoleQuery
{
    Task<Role?> GetByNameWithPermissionsAsync(string roleName, CancellationToken cancellationToken = default);
}
