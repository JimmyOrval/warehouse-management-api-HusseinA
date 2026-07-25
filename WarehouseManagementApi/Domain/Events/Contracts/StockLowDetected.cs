namespace Domain.Events.Contracts;

public record StockLowDetected : WarehouseEvent
{
    public required string ProductName { get; init; }
    public required int CurrentQuantity { get; init; }
    public required int MinimumQuantity { get; init; }
}