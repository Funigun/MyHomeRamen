namespace MyHomeRamen.Features.Common.Authorization;

public sealed record PermissionGrant(string Module, string Name, bool IsResourceScoped, Guid? RestaurantId);
