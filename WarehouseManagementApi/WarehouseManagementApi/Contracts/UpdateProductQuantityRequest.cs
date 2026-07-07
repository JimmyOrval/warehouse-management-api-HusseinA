using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementApi.Contracts;

public class UpdateProductQuantityRequest
{
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; set; }
    
    [Required(ErrorMessage = "Item location is required")]
    public string Location { get; set; }
}