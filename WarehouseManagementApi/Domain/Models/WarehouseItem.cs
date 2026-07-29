using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Models;

public class WarehouseItem
{
    [Key]
    public required string Id { get; init; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Product ID is required")]
    [Length(36, 36, ErrorMessage = "Product ID format invalid")]
    public required string ProductId { get; init; }
    public virtual Product? Product { get; init; }
    
    [Required(ErrorMessage = "Item location is required")]
    public required string Location { get; init; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; private set; } = 0;
    
    public DateTime LastStockUpdate { get; set; } = DateTime.UtcNow;

    private readonly List<StockMovement> _movements = [];
    public ICollection<StockMovement> Movements => _movements;
    
    public void StockIn(int quantity)
    {
        if(quantity <= 0)
            throw new BusinessRuleException("Quantity cannot be less than 1");
        
        QuantityInStock += quantity;
        LastStockUpdate = DateTime.UtcNow;
        
        _movements.Add(new StockMovement
        {
            Id = Guid.NewGuid().ToString(),
            WarehouseItemId = Id,
            MovementDate = DateTime.UtcNow,
            Quantity = quantity,
            MovementType = StockMovementType.StockIn
        });
    }

    public void StockOut(int quantity)
    {
        if(quantity <= 0)
            throw new BusinessRuleException("Quantity cannot be negative");
        
        if(quantity > QuantityInStock)
            throw new BusinessRuleException("Quantity insufficient");
        
        QuantityInStock -= quantity;
        LastStockUpdate = DateTime.UtcNow;
        
        _movements.Add(new StockMovement
        {
            Id = Guid.NewGuid().ToString(),
            WarehouseItemId = Id,
            MovementDate = DateTime.UtcNow,
            Quantity = quantity,
            MovementType = StockMovementType.StockOut
        });
    }
}