using MyHomeRamen.Domain.Restaurants.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Restaurants.ExternalApi;

public sealed class RestaurantsPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public string ModuleName => "Restaurants";

    public IReadOnlyCollection<PermissionDefinition> Permissions => PermissionConstants.AvailablePermissions
        .Select(permission => new PermissionDefinition(permission, permission))
        .ToArray();

    public IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; } = [];
}
