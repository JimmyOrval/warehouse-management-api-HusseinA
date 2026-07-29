using Domain.Events;

namespace Domain.Interfaces;

public interface INotificationConsumptionService
{
    Task<string> ConsumeAsync(
        WarehouseEvent warehouseEvent,
        CancellationToken cancellationToken);
}