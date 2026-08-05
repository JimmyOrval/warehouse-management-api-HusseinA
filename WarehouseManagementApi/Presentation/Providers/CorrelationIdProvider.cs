using Domain.Interfaces;

namespace Presentation.Providers;

public class CorrelationIdProvider(IHttpContextAccessor httpContextAccessor) : ICorrelationIdProvider
{
    public string CorrelationId()
    {
        return httpContextAccessor.HttpContext?.TraceIdentifier
            ?? Guid.NewGuid().ToString();
    }
}