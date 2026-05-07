using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyHomeRamen.Blazor.Features.Account.Common.Models;

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

        httpContext!.Request.Headers.Remove("guest_id");

        ProtectedBrowserStorageResult<string> guestId = await protectedLocalStore.GetAsync<string>("Guest session Id", "my_home_ramen_guest_id");

        if (guestId.Success)
        {
            httpContext!.Request.Headers.Add("guest_id", guestId.Value);
            return;
        }

        RegisterGuestResponse response = await accountApiClient.RegisterGuestAsync(cancellationToken);
        httpContext!.Request.Headers.Add("guest_id", response!.GuestId.ToString());
        await protectedLocalStore.SetAsync("Guest session Id", "my_home_ramen_guest_id", response!.GuestId.ToString());
    }
}
