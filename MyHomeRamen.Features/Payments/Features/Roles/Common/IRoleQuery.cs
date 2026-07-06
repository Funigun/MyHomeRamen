using MyHomeRamen.Domain.Payments.Roles;

namespace MyHomeRamen.Features.Payments.Features.Roles.Common;

public interface IRoleQuery
{
    Task<Role?> GetByNameWithPermissionsAsync(string name, CancellationToken cancellationToken);
}
