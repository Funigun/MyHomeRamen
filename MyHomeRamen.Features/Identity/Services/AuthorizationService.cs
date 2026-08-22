using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Services;

internal sealed class AuthorizationService(CurrentUser currentUser, IIdentityDbContext identityDbContext) : IAuthorizationService
{
    private const string GuestIdCookieName = "guest_id";

    public async Task AuthorizeUser(HttpContext context, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Claim> claims = context.User.Claims.ToArray();

        string id = claims.FirstOrDefault(claim => claim.Type == ClaimConstants.KeycloakIdClaim)?.Value ?? string.Empty;
        bool isAuthenticated = !string.IsNullOrEmpty(id);

        Guid? domainUserId = TryGetDomainUserId(claims);
        Guid userId = domainUserId ?? TryGetGuestId(context) ?? Guid.Empty;

        IReadOnlyCollection<string> permissions = domainUserId is not null
                                                ? (await identityDbContext.Permission.Query().ByUserId(domainUserId.Value, context.RequestAborted)).Select(p => p.Name).ToArray()
                                                : [];

#pragma warning disable S1854 // Unused assignments should be removed
        // CurrentUser is scoped service, so we can assign it here and it will be available for the rest of the request
        currentUser = new CurrentUser
        {
            IdentityId = id,
            UserId = userId,
            Claims = claims,
            IsAuthenticated = isAuthenticated,
            IsGuest = !isAuthenticated,
            Permissions = permissions,
        };
#pragma warning restore S1854 // Unused assignments should be removed
    }

    public async Task<ICurrentUser> ImpersonateSystemAccount(CancellationToken cancellationToken)
    {
        User user = await identityDbContext.User.Query().SystemAccount(cancellationToken);

        currentUser = new CurrentUser
        {
            IdentityId = user.KeycloakUserId ?? string.Empty,
            UserId = user.Id.Value,
            Claims = [],
            IsAuthenticated = true,
            IsGuest = false,
            Permissions = [],
        };

        return currentUser;
    }

    private static Guid? TryGetDomainUserId(IReadOnlyCollection<Claim> claims)
    {
        Claim? domainIdClaim = claims.FirstOrDefault(claim => claim.Type == ClaimConstants.DomainIdClaim);

        return Guid.TryParse(domainIdClaim?.Value, out Guid userId) ? userId : null;
    }

    private static Guid? TryGetGuestId(HttpContext context)
    {
        return context.Request.Cookies.TryGetValue(GuestIdCookieName, out string? guestIdString)
               && Guid.TryParse(guestIdString, out Guid parsedId)
               ? parsedId
               : null;
    }
}
