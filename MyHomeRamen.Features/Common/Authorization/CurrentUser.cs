using System.Security.Claims;

namespace MyHomeRamen.Features.Common.Authorization;

public sealed record CurrentUser : ICurrentUser
{
    public string IdentityId { get; private set; } = default!;

    public Guid UserId { get; private set; } = Guid.Empty;

    public IEnumerable<Claim> Claims { get; private set; } = [];

    public bool IsAuthenticated { get; private set; }

    public bool IsGuest { get; private set; }

    public IReadOnlyCollection<string> Permissions { get; private set; } = [];

    public IReadOnlyCollection<PermissionGrant> PermissionGrants { get; private set; } = [];

    internal void Update(
        string identityId,
        Guid userId,
        IEnumerable<Claim> claims,
        bool isAuthenticated,
        bool isGuest,
        IReadOnlyCollection<PermissionGrant> permissionGrants)
    {
        IdentityId = identityId;
        UserId = userId;
        Claims = claims;
        IsAuthenticated = isAuthenticated;
        IsGuest = isGuest;
        PermissionGrants = permissionGrants;
        Permissions = permissionGrants.Where(grant => grant.RestaurantId is null)
                                      .Select(grant => grant.Name)
                                      .Distinct(StringComparer.Ordinal)
                                      .ToArray();
    }

    public static CurrentUser Anonymous { get; } = new()
    {
        IdentityId = string.Empty,
        UserId = Guid.Empty,
        IsAuthenticated = false,
        IsGuest = true,
    };
}
