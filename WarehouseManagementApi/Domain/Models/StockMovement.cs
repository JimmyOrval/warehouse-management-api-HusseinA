using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models;

public class StockMovement
{
    [Key]
    public required string Id { get; init; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Item ID is required")]
    [Length(36, 36, ErrorMessage = "Item ID format invalid")]
    public required string WarehouseItemId { get; init; }
    public virtual WarehouseItem? WarehouseItem { get; init; }
    
    [Required(ErrorMessage = "Movement date is required")]
    public DateTime MovementDate { get; init; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; init; }
    
    [Required]
    public StockMovementType MovementType { get; init; }
}