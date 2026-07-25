namespace Domain.Interfaces;

public interface INotificationServiceClient
{
    // this is used for the HTTP bonus challenge
    Task<int?> GetUnreadNotificationCountAsync(CancellationToken cancellationToken);
}