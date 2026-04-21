using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyHomeRamen.Api.Common.Configuration;

namespace MyHomeRamen.Api.Common.Authorization;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor, RestaurantConfigurationProvider configurationProvider) : ICurrentUser
{
    private const string UserIdClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

    public string Id { get; init; } = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == UserIdClaim)?.Value
                                      ?? string.Empty;

    public Guid RestaurantId { get; init; } = configurationProvider.RestaurantId;

    public IEnumerable<Claim> Claims { get; init; } = httpContextAccessor.HttpContext?.User?.Claims ?? [];
}
