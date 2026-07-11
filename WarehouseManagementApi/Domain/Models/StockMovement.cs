using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models;

public class StockMovement
{
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Item ID is required")]
    public required string WarehouseItemId { get; set; }
    public WarehouseItem? WarehouseItem { get; set; }
    
    [Required(ErrorMessage = "Movement date is required")]
    public DateTime MovementDate { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    [Required]
    public StockMovementType MovementType { get; set; }
}