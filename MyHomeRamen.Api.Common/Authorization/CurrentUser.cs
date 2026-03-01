using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Api.Common.Authorization;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id { get; init; } = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? string.Empty;

    public Guid RestaurantId { get; init; } = Guid.TryParse(httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == "RestaurantId")?.Value, out Guid restaurantId) ? restaurantId : Guid.Empty;

    public IEnumerable<Claim> Claims { get; init; } = httpContextAccessor.HttpContext?.User?.Claims ?? [];
}
