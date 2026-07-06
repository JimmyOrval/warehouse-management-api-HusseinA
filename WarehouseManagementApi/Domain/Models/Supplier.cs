using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementApi.Models;

public class Supplier
{
    [Key]
    [Required]
    public required string Id { get; set; }
    
    [Required(ErrorMessage = "Supplier name is required")]
    [StringLength(100, ErrorMessage = "Supplier name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Country is required")]
    [StringLength(57, ErrorMessage = "Country cannot be longer than 57 characters")]
    public string Country { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Email address is not a valid email address")]
    [StringLength(255, ErrorMessage = "Email address cannot be longer than 255 characters")]
    public string ContactEmail { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone]
    [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters")]
    public string Phone { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}