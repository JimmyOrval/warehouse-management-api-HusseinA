using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Notifications.Queries.ListNotifications;

public class ListNotificationsQueryHandler(
    INotificationRepository notificationRepository,
    IMapper mapper,
    ILogger<ListNotificationsQueryHandler> logger)
    : IRequestHandler<ListNotificationsQuery, List<NotificationViewModel>>
{
    public async Task<List<NotificationViewModel>> Handle(ListNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetAllAsync(cancellationToken);
        logger.LogInformation("{NotificationCount} notifications retrieved", notifications.Count);
        return mapper.Map<List<NotificationViewModel>>(notifications);
    }
}