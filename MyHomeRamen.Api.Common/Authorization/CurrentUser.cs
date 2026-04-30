using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Common.Extentsions;

namespace MyHomeRamen.Api.Common.Authorization;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor, RestaurantConfigurationProvider configurationProvider) : ICurrentUser
{
    public string Id { get; init; } = httpContextAccessor.GetIdentityId();

    public Guid UserId => httpContextAccessor.TryGetUserId()
                       ?? httpContextAccessor.TryGetGuestId()
                       ?? Guid.Empty;

    public Guid RestaurantId { get; init; } = configurationProvider.RestaurantId;

    public IEnumerable<Claim> Claims { get; init; } = httpContextAccessor.HttpContext?.User?.Claims ?? [];
}
