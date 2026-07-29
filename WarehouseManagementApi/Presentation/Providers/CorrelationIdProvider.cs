using Domain.Interfaces;

namespace Presentation.Providers;

public class CorrelationIdProvider(HttpContextAccessor httpContextAccessor) : ICorrelationIdProvider
{
    public string CorrelationId()
    {
        return httpContextAccessor.HttpContext?.TraceIdentifier
            ?? Guid.NewGuid().ToString();
    }
}