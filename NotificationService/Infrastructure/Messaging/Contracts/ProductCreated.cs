using Domain.Models;

namespace Infrastructure.Messaging.Contracts;

public record ProductCreated : WarehouseEvent
{
    public required string ProductName { get; init; }
    public required string Sku { get; init; }
    public required string SupplierName { get; set; }
}