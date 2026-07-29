using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class NotificationRepository(NotificationDbContext context)
    : INotificationRepository
{
    public async Task<List<Notification>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification?> GetByIdAsync(string notificationId, CancellationToken cancellationToken)
    {
        return await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);
    }

    public async Task<Notification?> GetByEventIdAsync(string eventId, CancellationToken cancellationToken)
    {
        return await context.Notifications
            .FirstOrDefaultAsync(n => n.EventId == eventId, cancellationToken);
    }

    public void Add(Notification notification)
    {
        context.Notifications.Add(notification);
    }

    public async Task<int> CountByStatusAsync(NotificationStatus status, CancellationToken cancellationToken)
    {
        return await context.Notifications.CountAsync(
            n => n.Status == status, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}