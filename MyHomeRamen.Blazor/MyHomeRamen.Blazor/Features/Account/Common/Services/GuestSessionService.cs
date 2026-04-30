using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MyHomeRamen.Blazor.Features.Account.Common.Services;

public class GuestSessionService(CustomerAccountApiClient accountApiClient, IHttpContextAccessor httpContextAccessor, ProtectedLocalStorage protectedLocalStore)
{
    public async Task EnsureGuestSessionAsync(CancellationToken cancellationToken = default)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            return;
        }

        ProtectedBrowserStorageResult<string> guestId = await protectedLocalStore.GetAsync<string>("Guest session Id", "my_home_ramen_guest_id");

        if (guestId.Success)
        {
            return;
        }

        _ = await accountApiClient.RegisterGuestAsync(cancellationToken);

        if (httpContext.Request.Cookies.TryGetValue("guest_id", out string? guestIdString))
        {
            await protectedLocalStore.SetAsync("Guest session Id", "my_home_ramen_guest_id", guestIdString);
        }
    }
}
