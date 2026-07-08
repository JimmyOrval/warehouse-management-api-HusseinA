using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts;

public record UpdateProductPriceRequest
{
    [Required(ErrorMessage = "Price is required")]
    [Range(0.0, double.MaxValue, ErrorMessage =  "Price cannot be negative")]
    public decimal Price { get; set; }
}