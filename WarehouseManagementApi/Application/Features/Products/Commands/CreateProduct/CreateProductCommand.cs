using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<string>
{
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
    public decimal Price { get; init; }
    
    [Required(ErrorMessage = "Supplier ID is required")]
    [MinLength(36, ErrorMessage = "Supplier ID cannot be shorter than 36 characters")]
    [MaxLength(36, ErrorMessage = "Supplier ID cannot be longer than 36 characters")]
    public required string SupplierId { get; init; }
    
    [Required(ErrorMessage = "Expiry date is required")]
    public DateTime ExpiryDate { get; init; }
}