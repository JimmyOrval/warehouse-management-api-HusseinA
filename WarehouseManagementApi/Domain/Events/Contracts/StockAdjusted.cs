namespace Domain.Events.Contracts;

public record StockAdjusted : WarehouseEvent
{
    public required string ProductName { get; init; }
    public required int PreviousQuantity { get; init; }
    public required int NewQuantity { get; init; }
}