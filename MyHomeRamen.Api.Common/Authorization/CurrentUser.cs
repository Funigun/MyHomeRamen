using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Api.Common.Authorization;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id { get; init; } = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? string.Empty;
}
