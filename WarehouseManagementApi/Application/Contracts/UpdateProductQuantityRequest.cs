using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts;

public record UpdateProductQuantityRequest
{
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    [Required(ErrorMessage = "Item location is required")]
    public required string Location { get; set; }
}