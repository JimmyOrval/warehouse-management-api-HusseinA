using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class WarehouseItem
{
    [Key]
    public required string Id { get; set; }
    
    [Required(ErrorMessage = "Product connection is required")]
    public required string ProductId { get; set; }
    public Product? Product { get; set; }
    
    [Required(ErrorMessage = "Item location is required")]
    public string Location { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; set; } = 0;
    
    public DateTime LastStockUpdate { get; set; }
    
    public void StockIn(int quantity)
    {
        QuantityInStock += quantity;
        LastStockUpdate = DateTime.Now;
    }

    public void StockOut(int quantity)
    {
        QuantityInStock -= quantity;
        LastStockUpdate = DateTime.Now;
    }
}