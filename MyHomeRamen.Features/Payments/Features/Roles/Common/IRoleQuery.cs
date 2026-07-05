using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Features.Payments.Features.Roles.Common;

public interface IRoleQuery
{
    Task<Role?> GetByNameWithPermissionsAsync(string name, CancellationToken cancellationToken);
}
