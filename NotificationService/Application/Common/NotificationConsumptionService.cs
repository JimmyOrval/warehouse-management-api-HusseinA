using Application.Features.Notifications.Commands.CreateNotificationFromEvent;
using Domain.Events;
using Domain.Interfaces;
using MediatR;

namespace Application.Common;

public class NotificationConsumptionService(IMediator mediator)
    : INotificationConsumptionService
{
    public async Task<string> ConsumeAsync(WarehouseEvent warehouseEvent, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateNotificationFromEventCommand(
                warehouseEvent.EventId,
                warehouseEvent.Type,
                warehouseEvent.Title,
                warehouseEvent.Message,
                warehouseEvent.Severity,
                warehouseEvent.RelatedEntityId,
                warehouseEvent.RelatedEntity),
            cancellationToken);
    }
}