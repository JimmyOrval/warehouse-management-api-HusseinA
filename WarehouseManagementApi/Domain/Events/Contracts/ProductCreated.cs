namespace Domain.Events.Contracts;

public record ProductCreated : WarehouseEvent
{
    public required string ProductName { get; init; }
    public required string Sku { get; init; }
    public required string SupplierName { get; init; }
}