using MyHomeRamen.Blazor.Features.Account.Common.Services;

namespace MyHomeRamen.Blazor.Presentation.GuestAuthentication;

public class GuestSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, GuestSessionService guestSessionService)
    {
        await guestSessionService.EnsureGuestSessionAsync(context.RequestAborted);
        await next(context);
    }
}
