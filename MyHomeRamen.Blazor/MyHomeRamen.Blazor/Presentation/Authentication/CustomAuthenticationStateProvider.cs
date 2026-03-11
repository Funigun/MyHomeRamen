using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace MyHomeRamen.Blazor.Presentation.Authentication;

public sealed class CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext?.User
            ?? new ClaimsPrincipal(new ClaimsIdentity());

        return Task.FromResult(new AuthenticationState(user));
    }

    public async Task<ClaimsPrincipal> GetCurrentUserAsync()
    {
        AuthenticationState state = await GetAuthenticationStateAsync();
        return state.User;
    }

    public async Task<string?> GetCurrentUserIdAsync()
    {
        ClaimsPrincipal user = await GetCurrentUserAsync();
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<string?> GetCurrentUserEmailAsync()
    {
        ClaimsPrincipal user = await GetCurrentUserAsync();
        return user.FindFirstValue(ClaimTypes.Email);
    }

    public async Task<string?> GetCurrentUserNameAsync()
    {
        ClaimsPrincipal user = await GetCurrentUserAsync();
        return user.FindFirstValue("preferred_username")
            ?? user.FindFirstValue(ClaimTypes.Name);
    }

    public async Task<IEnumerable<string>> GetCurrentUserRolesAsync()
    {
        ClaimsPrincipal user = await GetCurrentUserAsync();
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
}
