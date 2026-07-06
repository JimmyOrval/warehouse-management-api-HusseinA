using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Domain.Models;

public class Product
{
    [Key]
    [Required]
    public required string Id { get; init; }
    
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50, ErrorMessage = "Product name cannot be longer than 50 characters")]
    public required string Name { get; init; }
    
    [Required(ErrorMessage = "Product SKU is required")]
    [StringLength(50, ErrorMessage = "Product SKU cannot be longer than 50 characters")]
    public required string Sku { get; init; }
    
    [MaxLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
    public required string Description { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.1, double.MaxValue, ErrorMessage =  "Price cannot be negative")]
    public decimal Price { get; set; } = 0.00m;
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; set; } = 0;
    
    [Required(ErrorMessage = "Supplier ID is required")]
    [MinLength(36, ErrorMessage = "Supplier ID cannot be shorter than 36 characters")]
    [MaxLength(36, ErrorMessage = "Supplier ID cannot be longer than 36 characters")]
    public required string SupplierId { get; set; }
    
    public Supplier? Supplier { get; set; }
    
    [Required(ErrorMessage = "Expiry date is required")]
    public DateTime ExpiryDate { get; set; }
    
    public bool IsArchived { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; init; } =  DateTime.Now;

    public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

    public void Archive()
    {
        if(IsArchived)
            throw new InvalidOperationException("Product already archived");
        IsArchived = true;
        LastUpdatedAt = DateTime.Now;
    }

    public void AssignSupplier(Supplier supplier)
    {
        if(!supplier.IsActive)
            throw new ArgumentException("Supplier is not active");
        SupplierId = supplier.Id;
        LastUpdatedAt = DateTime.Now;
    }
}