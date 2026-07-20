using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infrastructure.HealthChecks;

public class RedisRetryHealthCheck(
    IConnectionMultiplexer redis,
    ILogger<RedisRetryHealthCheck> logger) : IHealthCheck
{
    private const int MaxRetries = 3;
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        Exception? exception = null;
        
        for(var retry = 1; retry <= MaxRetries; retry++)
        {
            try
            {
                var db = redis.GetDatabase();
                var latency = await db.PingAsync();

                if (retry > 1)
                {
                    logger.LogInformation("Redis health check succeeded on attempt {retry}/{MaxRetries}",
                        retry, MaxRetries);
                }

                return HealthCheckResult.Healthy(
                    $"Redis responded in {latency} ms. (retry {retry}/{MaxRetries})");
            }

            catch (Exception ex) when (retry < MaxRetries)
            {
                logger.LogWarning(ex,
                    "Redis health check attempt {retry}/{MaxRetries} failed, retrying", retry, MaxRetries);

                await Task.Delay(TimeSpan.FromMilliseconds(200 * retry), cancellationToken);
            }

            catch (Exception ex)
            {
                exception = ex;
                
                logger.LogError(ex,
                    "Redis health check failed after {MaxRetries} attempts", MaxRetries);

                return HealthCheckResult.Unhealthy(
                    $"Redis unreachable after {MaxRetries} attempts", ex);
            }
        } 
        return HealthCheckResult.Unhealthy(
        $"Redis unreachable after {MaxRetries} retries.", exception);
    }
}