using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler(
    INotificationRepository notificationRepository,
    IMapper mapper,
    ILogger<MarkNotificationAsReadCommandHandler> logger)
    : IRequestHandler<MarkNotificationAsReadCommand, NotificationViewModel>
{
    public async Task<NotificationViewModel> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if(notification == null)
        {
            logger.LogWarning("Notification {NotificationId} not found", request.Id);
            throw new NotFoundException($"Notification {request.Id} not found");
        }
        
        notification.MarkAsRead();
        await notificationRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Notification {NotificationId} marked as read", notification.Id);
        return mapper.Map<NotificationViewModel>(notification);
    }
}