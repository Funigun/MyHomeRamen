using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.Features.Common.Authorization;

public sealed record PermissionGrant(
    string Module,
    string Name,
    bool IsResourceScoped,
    RoleScope Scope,
    Guid? RestaurantId);
