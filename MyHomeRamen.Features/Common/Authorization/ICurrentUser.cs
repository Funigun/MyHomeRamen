using System.Security.Claims;

namespace MyHomeRamen.Features.Common.Authorization;

public interface ICurrentUser
{
    string IdentityId { get; }

    Guid UserId { get; }

    IEnumerable<Claim> Claims { get; }

    bool IsAuthenticated { get; }

    bool IsGuest { get; }

    IReadOnlyCollection<string> Permissions { get; }

    IReadOnlyCollection<PermissionGrant> PermissionGrants { get; }

    bool HasPermission(string module, string permission, Guid? restaurantId = null)
        => PermissionGrants.Any(grant =>
            grant.Module == module
            && grant.Name == permission
            && (grant.RestaurantId is null
                || (grant.IsResourceScoped && restaurantId.HasValue && grant.RestaurantId == restaurantId)));

    bool HasAnyPermission(string module, string permission)
        => PermissionGrants.Any(grant =>
            grant.Module == module
            && grant.Name == permission
            && (grant.RestaurantId is null || grant.IsResourceScoped));

    IReadOnlyCollection<Guid> RestaurantIdsFor(string module, string permission)
        => PermissionGrants.Where(grant =>
                grant.Module == module
                && grant.Name == permission
                && grant.IsResourceScoped
                && grant.RestaurantId.HasValue)
            .Select(grant => grant.RestaurantId!.Value)
            .Distinct()
            .ToArray();
}
