using Domain.Enums;

namespace Application.ViewModels;

public class StockMovementViewModel
{
    public required string Id { get; init; }
    
    public required string WarehouseItemId { get; init; }
    
    public string? WarehouseItemName { get; init; }

    public DateTime MovementDate { get; init; }

    public int Quantity { get; init; }

    public StockMovementType MovementType { get; init; }
}