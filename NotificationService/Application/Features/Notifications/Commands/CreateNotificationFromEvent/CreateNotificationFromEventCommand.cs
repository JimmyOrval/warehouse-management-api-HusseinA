using MediatR;

namespace Application.Features.Notifications.Commands.CreateNotificationFromEvent;

public record CreateNotificationFromEventCommand(
    string EventId,
    string Type,
    string Title,
    string Message,
    string Severity,
    string RelatedEntityId,
    string RelatedEntity) : IRequest<string>;