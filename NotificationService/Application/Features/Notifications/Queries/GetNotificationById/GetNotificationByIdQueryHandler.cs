using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Notifications.Queries.GetNotificationById;

public class GetNotificationByIdQueryHandler(
    INotificationRepository notificationRepository,
    IMapper mapper,
    ILogger<GetNotificationByIdQueryHandler> logger)
    : IRequestHandler<GetNotificationByIdQuery, NotificationViewModel>
{
    public async Task<NotificationViewModel> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (notification == null)
        {
            logger.LogWarning("Notification {NotificationId} not found", request.Id);
            throw new NotFoundException($"Notification {request.Id} not found");
        }
        
        logger.LogInformation("Notification {NotificationId} retrieved", request.Id);
        return mapper.Map<NotificationViewModel>(notification);
    }
}