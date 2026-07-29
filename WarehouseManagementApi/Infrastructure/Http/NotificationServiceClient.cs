using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Http;

public class NotificationServiceClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor,
    ILogger<NotificationServiceClient> logger)
    : INotificationServiceClient
{
    public async Task<int?> GetUnreadNotificationCountAsync(CancellationToken cancellationToken)
    {
        try
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization
                .ToString().Replace("Bearer ", "");

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/notifications/unread-count");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            
            var response = await httpClient.SendAsync(
                request,
                cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Notification Service returned {StatusCode} for unread count",
                    response.StatusCode);
                return null;
            }

            var result = await response.Content
                .ReadFromJsonAsync<UnreadCountResponse>(
                    cancellationToken);
            return result?.Count;
        }
        catch (TaskCanceledException)
        {
            logger.LogWarning("Notification Service call timed out.");
            return null;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Notification Service unreachable.");
            return null;
        }
    }

    private record UnreadCountResponse(int Count);
}