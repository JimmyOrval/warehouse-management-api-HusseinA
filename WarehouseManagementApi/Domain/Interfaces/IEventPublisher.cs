namespace Domain.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(
        TEvent warehouseEvent,
        string routingKey,
        CancellationToken cancellationToken)
        where TEvent : Events.WarehouseEvent;
}