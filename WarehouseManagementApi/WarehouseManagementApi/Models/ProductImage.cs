using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementApi.Models;

public class ProductImage
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string FileName { get; set; }
    [Required]
    public string FilePath { get; set; }
}