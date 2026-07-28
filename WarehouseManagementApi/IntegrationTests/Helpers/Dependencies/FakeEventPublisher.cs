using Domain.Events;
using Domain.Interfaces;

namespace IntegrationTests.Helpers.Dependencies;

public class FakeEventPublisher : IEventPublisher
{
    // fake publisher to replace the real publisher
    public Task PublishAsync<TEvent>(TEvent warehouseEvent, string routingKey, CancellationToken cancellationToken)
    where TEvent : WarehouseEvent
    {
        // we don't have to actually publish the real event
        // as long as the method returns that it was published
        return Task.CompletedTask;
    }
}