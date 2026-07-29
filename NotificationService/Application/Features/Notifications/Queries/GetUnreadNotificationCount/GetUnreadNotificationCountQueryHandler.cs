using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Notifications.Queries.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler(INotificationRepository notificationRepository)
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken ct) =>
        await notificationRepository.CountByStatusAsync(NotificationStatus.Unread, ct);
}