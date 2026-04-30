namespace MyHomeRamen.Blazor.Presentation.GuestAuthentication;

public class GuestCookieForwardingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;

        if (httpContext is not null &&
            httpContext.Request.Cookies.TryGetValue("guest_id", out string? guestId) &&
            !string.IsNullOrWhiteSpace(guestId))
        {
            request.Headers.TryAddWithoutValidation("Cookie", $"guest_id={guestId}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
