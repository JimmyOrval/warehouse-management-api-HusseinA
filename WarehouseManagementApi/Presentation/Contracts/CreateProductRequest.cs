using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts;

public record CreateProductRequest
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
    
    [Required(ErrorMessage = "Supplier ID is required")]
    [MinLength(36, ErrorMessage = "Supplier ID cannot be shorter than 36 characters")]
    [MaxLength(36, ErrorMessage = "Supplier ID cannot be longer than 36 characters")]
    public required string SupplierId { get; set; }
    
    [Required(ErrorMessage = "Expiry date is required")]
    public DateTime ExpiryDate { get; set; }
}