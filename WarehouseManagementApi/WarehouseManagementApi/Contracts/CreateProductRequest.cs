using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementApi.Contracts;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(50, ErrorMessage = "Product name cannot be longer than 50 characters")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Product SKU is required")]
    [StringLength(50, ErrorMessage = "Product SKU cannot be longer than 50 characters")]
    public required string Sku { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
    public required string Description { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
    public decimal Price { get; set; } = 0.00m;
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; set; } = 0;
    
    [Required(ErrorMessage = "Supplier name is required")]
    [StringLength(50, ErrorMessage = "Supplier name cannot be longer than 50 characters")]
    public required string SupplierName { get; set; }
    
    [Required(ErrorMessage = "Expiry date is required")]
    public DateTime ExpiryDate { get; set; }
}