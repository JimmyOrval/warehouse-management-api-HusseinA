using System.ComponentModel.DataAnnotations;

namespace Application.Contracts;

public class UpdateProductPriceRequest
{
    [Required(ErrorMessage = "Price is required")]
    [Range(0.0, double.MaxValue, ErrorMessage =  "Price cannot be negative")]
    public decimal Price { get; set; }
}