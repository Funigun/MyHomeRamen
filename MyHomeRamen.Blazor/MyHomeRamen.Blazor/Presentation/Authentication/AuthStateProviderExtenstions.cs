using Microsoft.AspNetCore.Components.Authorization;

namespace MyHomeRamen.Blazor.Presentation.Authentication;

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
            return roles.Contains("Admin");
        }

        public async Task<bool> IsEmployee()
        {
            IEnumerable<string> roles = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserRolesAsync();
            return roles.Contains("Employee");
        }

        public async Task<bool> IsCustomer()
        {
            IEnumerable<string> roles = await ((CustomAuthenticationStateProvider)authenticationState).GetCurrentUserRolesAsync();
            return roles.Contains("Customer");
        }
    }
}
