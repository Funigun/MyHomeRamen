using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MyHomeRamen.Api.Common.Middleware;

public sealed class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Handling request");

        await next(context);

        logger.LogInformation("Finished handling request");
    }
}
