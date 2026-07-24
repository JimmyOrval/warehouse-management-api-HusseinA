using Domain.Models;

namespace Domain.Interfaces;

public interface INotificationConsumptionService
{
    Task<string> ConsumeAsync(
        WarehouseEvent warehouseEvent,
        CancellationToken cancellationToken);
}