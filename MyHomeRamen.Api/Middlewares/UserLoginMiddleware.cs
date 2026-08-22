using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Api.Middlewares;

public sealed class UserLoginMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAuthorizationService authorizationService)
    {
        await authorizationService.AuthorizeUser(context, context.RequestAborted);

        await next(context);
    }
}
