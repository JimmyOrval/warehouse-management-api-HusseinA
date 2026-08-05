using Domain.Enums;

namespace Application.ViewModels;

public class ShipmentViewModel
{
    public required string Id { get; init; }
    public required string SupplierId { get; init; }
    public string? SupplierName { get; init; }
    public ShipmentStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<ShipmentItemViewModel> Items { get; init; } = [];
}

public class ShipmentItemViewModel
{
    public required string ProductId { get; init; }
    public string? ProductName { get; init; }
    public int Quantity { get; init; }
}
