using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Notifications.Commands.CreateNotificationFromEvent;

public class CreateNotificationFromEventCommandHandler(
    INotificationRepository notificationRepository,
    ILogger<CreateNotificationFromEventCommandHandler> logger)
    : IRequestHandler<CreateNotificationFromEventCommand, string>
{
    public async Task<string> Handle(CreateNotificationFromEventCommand request, CancellationToken cancellationToken)
    {
        var existingNotification = await notificationRepository.GetByEventIdAsync(
            request.EventId, cancellationToken);
        if(existingNotification != null)
        {
            logger.LogWarning("Notification for event {EventId} already exists.", request.EventId);
            return existingNotification!.Id;
        }

        var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                EventId = request.EventId,
                Type = Enum.Parse<NotificationType>(request.Type),
                Title = request.Title,
                Message = request.Message,
                Severity = Enum.Parse<NotificationSeverity>(request.Severity),
                RelatedEntityId = request.RelatedEntityId,
                RelatedEntity = Enum.Parse<RelatedEntity>(request.RelatedEntity),
                Status = NotificationStatus.Unread
            };
        notificationRepository.Add(notification);
        await notificationRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Notification successfully created.");
        return notification.Id;
    }
}