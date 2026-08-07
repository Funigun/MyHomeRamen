using System.Diagnostics;

namespace MyHomeRamen.Api.Middlewares;

public sealed class PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
{
    private readonly TimeSpan _threshold = TimeSpan.FromSeconds(1);

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;

            if (elapsed.TotalMilliseconds > _threshold.TotalMilliseconds)
            {
                logger.LogWarning
                (
                    "Request {RequestPath} took {ElapsedMilliseconds}ms which exceeds the threshold of {ThresholdMilliseconds}ms",
                    context.Request.Path,
                    elapsed.TotalMilliseconds,
                    _threshold.TotalMilliseconds
                );
            }
        }
    }
}
