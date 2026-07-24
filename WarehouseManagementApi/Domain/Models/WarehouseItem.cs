using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;

namespace Domain.Models;

public class WarehouseItem
{
    [Key]
    public required string Id { get; init; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Product connection is required")]
    public required string ProductId { get; init; }
    public virtual Product? Product { get; init; }
    
    [Required(ErrorMessage = "Item location is required")]
    public string Location { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; set; } = 0;
    
    public DateTime LastStockUpdate { get; set; }
    
    public void StockIn(int quantity)
    {
        QuantityInStock += quantity;
        LastStockUpdate = DateTime.UtcNow;
    }

    public void StockOut(int quantity)
    {
        if(quantity <= 0)
            throw new BusinessRuleException("Quantity cannot be negative");
        
        if(quantity > QuantityInStock)
            throw new BusinessRuleException("Quantity insufficient");
        
        QuantityInStock -= quantity;
        LastStockUpdate = DateTime.UtcNow;
    }
}