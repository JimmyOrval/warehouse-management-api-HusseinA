using Serilog.Context;

namespace Presentation.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId;

        if (context.Request.Headers.TryGetValue(HeaderName, out var existing))
            correlationId = existing!;
        else
            correlationId = Guid.NewGuid().ToString();
        
        context.TraceIdentifier = correlationId;
        context.Response.Headers.Append(HeaderName, correlationId);
        
        using(LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}