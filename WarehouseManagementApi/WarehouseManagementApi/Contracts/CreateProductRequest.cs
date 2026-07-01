using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementApi.Contracts;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(50, ErrorMessage = "Product name cannot be longer than 50 characters")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Product sku is required")]
    public string Sku { get; set; }
    [MaxLength(100, ErrorMessage = "Product sku cannot be longer than 100 characters")]
    public string Description { get; set; }
    public decimal Price { get; set; } = 0.00m;
    public int QuantityInStock { get; set; } = 0;
    public string SupplierName { get; set; }
    public DateTime ExpiryDate { get; set; }
}