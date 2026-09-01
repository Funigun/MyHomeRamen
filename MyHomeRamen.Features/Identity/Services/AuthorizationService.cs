using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Services;

internal sealed class AuthorizationService(CurrentUser currentUser, IIdentityDbContext identityDbContext, ILogger<AuthorizationService> logger) : IAuthorizationService
{
    private const string GuestIdCookieName = "guest_id";

    public async Task AuthorizeUser(HttpContext context, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Claim> claims = context.User.Claims.ToArray();

        string identityId = claims.FirstOrDefault(claim => claim.Type == ClaimConstants.KeycloakIdClaim)?.Value ?? string.Empty;
        bool isAuthenticated = !string.IsNullOrEmpty(identityId);

        Guid? domainUserId = TryGetDomainUserId(claims);
        Guid? guestId = TryGetGuestId(claims, context);

        User? user = domainUserId is not null
                   ? await identityDbContext.User.Query().ById(domainUserId.Value, cancellationToken)
                   : guestId is not null
                       ? await identityDbContext.User.Query().ByGuestId(guestId.Value, cancellationToken)
                       : null;

        ValidateUser(identityId, user);

        IReadOnlyCollection<string> permissions = user is not null
                                                ? (await identityDbContext.Permission.Query().ByUserId(user.Id.Value, context.RequestAborted)).Select(p => p.Name).ToArray()
                                                : [];

        currentUser.Update(identityId, user?.Id.Value ?? Guid.Empty, claims, isAuthenticated, !isAuthenticated, permissions);
    }

    public async Task<ICurrentUser> ImpersonateSystemAccount(CancellationToken cancellationToken)
    {
        User user = await identityDbContext.User.Query().SystemAccount(cancellationToken);

        currentUser.Update(
            user.KeycloakUserId ?? string.Empty,
            user.Id.Value,
            [],
            true,
            false,
            []);

        return currentUser;
    }

    private static Guid? TryGetDomainUserId(IReadOnlyCollection<Claim> claims)
    {
        Claim? domainIdClaim = claims.FirstOrDefault(claim => claim.Type == ClaimConstants.DomainIdClaim);

        return Guid.TryParse(domainIdClaim?.Value, out Guid userId) ? userId : null;
    }

    private static Guid? TryGetGuestId(IReadOnlyCollection<Claim> claims, HttpContext context)
    {
        string? guestIdValue = claims.FirstOrDefault(claim => claim.Type == ClaimConstants.GuestIdClaim)?.Value;

        if (Guid.TryParse(guestIdValue, out Guid guestId))
        {
            return guestId;
        }

        return context.Request.Cookies.TryGetValue(GuestIdCookieName, out guestIdValue)
               && Guid.TryParse(guestIdValue, out guestId)
               ? guestId
               : null;
    }

    private void ValidateUser(string identityId, User? user)
    {
        if (!string.IsNullOrEmpty(identityId) && identityId != user?.KeycloakUserId)
        {
            logger.LogCritical("User identity mismatch between Keycloak and domain user.");
            throw new UnauthorizedAccessException();
        }
    }
}
