using Domain.Models;

namespace Domain.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<Notification?> GetByIdAsync(string notificationId, CancellationToken cancellationToken);
    
    Task<Notification?> GetByEventIdAsync(string eventId, CancellationToken cancellationToken);

    void Add(Notification notification);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}