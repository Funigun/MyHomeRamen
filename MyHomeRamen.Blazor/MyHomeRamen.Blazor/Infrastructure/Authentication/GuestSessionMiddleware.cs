using MyHomeRamen.Blazor.Features.Account.Common.Services;

namespace MyHomeRamen.Blazor.Infrastructure.Authentication;

public class GuestSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, GuestSessionService guestSessionService)
    {
        await guestSessionService.EnsureGuestSessionAsync(context.RequestAborted);
        await next(context);
    }
}
