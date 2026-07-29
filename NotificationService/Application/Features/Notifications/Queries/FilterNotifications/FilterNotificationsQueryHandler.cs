using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Notifications.Queries.FilterNotifications;

public class FilterNotificationsQueryHandler(
    INotificationRepository notificationRepository,
    IMapper mapper,
    ILogger<FilterNotificationsQueryHandler> logger)
    : IRequestHandler<FilterNotificationsQuery, List<NotificationViewModel>>
{
    public async Task<List<NotificationViewModel>> Handle(FilterNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetAllAsync(cancellationToken);
        
        var filteredNotifications = notifications
            .Where(n => request.Type == null || n.Type == request.Type)
            .Where(n => request.Severity == null || n.Severity == request.Severity)
            .Where(n => request.Status == null || n.Status == request.Status)
            .ToList();
        
        return mapper.Map<List<NotificationViewModel>>(filteredNotifications);
    }
}