namespace Infrastructure.Messaging.Contracts;

public record StockAdjusted : WarehouseIntegrationEvent
{
    public required string ProductName { get; init; }
    public required int PreviousQuantity { get; init; }
    public required int NewQuantity { get; init; }
}