namespace Infrastructure.Messaging.Contracts;

public record ProductCreated : WarehouseIntegrationEvent
{
    public required string ProductName { get; init; }
    public required string Sku { get; init; }
    public required string SupplierName { get; set; }
}