using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Models;

public class Product
{
    [Key]
    [Required]
    public required string Id { get; init; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50, ErrorMessage = "Product name cannot be longer than 50 characters")]
    public required string Name { get; init; }
    
    [Required(ErrorMessage = "Product SKU is required")]
    [StringLength(50, ErrorMessage = "Product SKU cannot be longer than 50 characters")]
    public required string Sku { get; init; }
    
    [MaxLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
    public required string Description { get; init; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.1, double.MaxValue, ErrorMessage =  "Price cannot be negative")]
    public decimal Price { get; set; } = 0.01m;
    
    [Required(ErrorMessage = "Supplier ID is required")]
    [MinLength(36, ErrorMessage = "Supplier ID cannot be shorter than 36 characters")]
    [MaxLength(36, ErrorMessage = "Supplier ID cannot be longer than 36 characters")]
    public required string SupplierId { get; set; }
    
    public virtual Supplier? Supplier { get; init; }
    
    [Required(ErrorMessage = "Expiry date is required")]
    public DateTime ExpiryDate { get; init; }
    
    // set is private since only domain logic can update it
    public ProductStatus Status { get; private set; } = ProductStatus.Active;
    
    [Required]
    public DateTime CreatedAt { get; init; } =  DateTime.UtcNow;

    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    public void Archive()
    {
        if(Status == ProductStatus.Archived)
            throw new BusinessRuleException("Product already archived");
        
        Status = ProductStatus.Archived;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void AssignSupplier(Supplier supplier)
    {
        if(!supplier.IsActive)
            throw new BusinessRuleException("Supplier is not active");
        
        if(SupplierId == supplier.Id)
            throw new BusinessRuleException("Supplier is already assigned");
        
        SupplierId = supplier.Id;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void ChangePrice(decimal newPrice)
    {
        if(newPrice<=0)
            throw new ValidationException("Price must be greater than 0");
        
        if(Status != ProductStatus.Active)
            throw new BusinessRuleException("Product is not available");
        
        Price = newPrice;
        LastUpdatedAt = DateTime.UtcNow;
    }
}