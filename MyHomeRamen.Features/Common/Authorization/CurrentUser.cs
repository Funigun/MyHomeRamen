using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Features.Common.Authorization;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id => GetIdentityId();

    public Guid UserId => TryGetUserId() ?? TryGetGuestId() ?? Guid.Empty;

    public IEnumerable<Claim> Claims { get; init; } = httpContextAccessor.HttpContext?.User?.Claims ?? [];

    private string GetIdentityId()
    {
        return httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimConstants.KeycloakIdClaim)?.Value
               ?? string.Empty;
    }

    private Guid? TryGetUserId()
    {
        Claim? domainIdClaim = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimConstants.DomainIdClaim);

        return Guid.TryParse(domainIdClaim?.Value, out Guid userId)
             ? userId
             : null;
    }

    private Guid? TryGetGuestId()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        return
            httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("guest_id", out string? guestIdString)
            && Guid.TryParse(guestIdString, out Guid parsedId)
            ? parsedId
            : null;
    }
}
