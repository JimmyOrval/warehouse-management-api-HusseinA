namespace Infrastructure.Messaging.Contracts;

public record StockLowDetected : WarehouseIntegrationEvent
{
    public required string ProductName { get; init; }
    public required int CurrentQuantity { get; init; }
    public required int MinimumQuantity { get; init; }
}