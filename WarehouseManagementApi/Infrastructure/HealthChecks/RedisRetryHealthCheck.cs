using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Infrastructure.HealthChecks;

public class RedisRetryHealthCheck(IConnectionMultiplexer redis) : IHealthCheck
{
    private const int MaxRetries = 3;
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        Exception? exception = null;
        
        for (var retry = 1; retry <= MaxRetries; retry++)
        {
            try
            {
                var db = redis.GetDatabase();
                var latency = await db.PingAsync();
                return HealthCheckResult.Healthy(
                    $"Redis responded in {latency} ms. (retry {retry}/{MaxRetries})");
            }

            catch (Exception ex)
            {
                exception = ex;

                if (retry < MaxRetries)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(200 * retry), cancellationToken);
                }
            }
            
        } 
        return HealthCheckResult.Unhealthy(
        $"Redis unreachable after {MaxRetries} retries.", exception);
    }
}