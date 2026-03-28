using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyHomeRamen.Api.Common.Configuration;

namespace MyHomeRamen.Api.Common.Authorization;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor, RestaurantConfigurationProvider configurationProvider) : ICurrentUser
{
    public string Id { get; init; } = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? string.Empty;

    public Guid RestaurantId { get; init; } = configurationProvider.RestaurantId;

    public IEnumerable<Claim> Claims { get; init; } = httpContextAccessor.HttpContext?.User?.Claims ?? [];
}
