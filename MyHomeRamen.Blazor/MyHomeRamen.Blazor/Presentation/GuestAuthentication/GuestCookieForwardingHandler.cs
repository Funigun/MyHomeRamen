using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Primitives;
using MyHomeRamen.Blazor.Presentation.Authentication;

namespace MyHomeRamen.Blazor.Presentation.GuestAuthentication;

public class GuestCookieForwardingHandler(IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authenticationState) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;

        if (await ShouldAddGuestCookie(httpContext))
        {
            httpContext!.Request.Headers.TryGetValue("guest_id", out StringValues guestId);
            request.Headers.TryAddWithoutValidation("Cookie", $"guest_id={guestId.First()}");
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<bool> ShouldAddGuestCookie(HttpContext? httpContext)
    {
        bool isAuthenticated = await authenticationState.IsAuthenticated();

        return !isAuthenticated &&
               httpContext is not null &&
               httpContext.Request.Headers.Any(header => header.Key == "guest_id");
    }
}
