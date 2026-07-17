using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand : IRequest<string>
{
    [Required(ErrorMessage = "Supplier name is required")]
    [StringLength(100, ErrorMessage = "Supplier name cannot be longer than 100 characters")]
    public required string Name { get; init; }
    
    [Required(ErrorMessage = "Country is required")]
    [StringLength(57, ErrorMessage = "Country cannot be longer than 57 characters")]
    public required string Country { get; init; }
    
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Email address is not a valid email address")]
    [StringLength(255, ErrorMessage = "Email address cannot be longer than 255 characters")]
    public required string ContactEmail { get; init; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone]
    [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters")]
    public required string Phone { get; init; }
}