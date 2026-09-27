using Microsoft.AspNetCore.Components.Authorization;

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
            IEnumerable<string> roles = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserRolesAsync();
            return roles.Contains("Restaurant Admin");
        }

        public async Task<bool> IsCompanyOwner()
        {
            IEnumerable<string> roles = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserRolesAsync();
            return roles.Contains("Company Owner");
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
