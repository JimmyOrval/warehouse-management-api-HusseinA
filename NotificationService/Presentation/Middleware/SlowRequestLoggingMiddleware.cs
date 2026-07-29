using System.Diagnostics;

namespace Presentation.Middleware;

public class SlowRequestLoggingMiddleware(RequestDelegate next, ILogger<SlowRequestLoggingMiddleware> logger)
{
    private const long MaxTime = 500;
    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await next(context);
        stopwatch.Stop();
        
        if(stopwatch.ElapsedMilliseconds > MaxTime)
        {
            logger.LogWarning(
                "Slow request: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms.",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}