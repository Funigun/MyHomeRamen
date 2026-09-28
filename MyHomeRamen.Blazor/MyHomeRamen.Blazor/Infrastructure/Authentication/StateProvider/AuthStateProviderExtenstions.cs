using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MyHomeRamen.Blazor.Infrastructure.Authentication;

namespace MyHomeRamen.Blazor.Infrastructure.Authentication.StateProvider;

public static class AuthStateProviderExtenstions
{
    extension(AuthenticationStateProvider authenticationState)
    {
        public async Task<string> GetUserName()
        {
            return await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserNameAsync() ?? string.Empty;
        }

        public async Task<bool> IsAuthenticated()
        {
            return await ((CustomAuthenticationStateProvider)authenticationState).IsAuthenticated();
        }

        public async Task<bool> IsAdmin()
        {
            ClaimsPrincipal user = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserAsync();
            return user.HasClaim(AuthenticationClaimConstants.AdminCanViewPanel, bool.TrueString);
        }

        public async Task<bool> IsCompanyOwner()
        {
            ClaimsPrincipal user = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserAsync();
            return user.HasClaim(AuthenticationClaimConstants.OwnerCanViewPanel, bool.TrueString);
        }

        public async Task<bool> IsGuest()
        {
            IEnumerable<string> roles = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserRolesAsync();
            return roles.Contains("Guest");
        }

        public async Task<bool> IsCustomer()
        {
            IEnumerable<string> roles = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserRolesAsync();
            return roles.Contains("Customer");
        }
    }
}
