using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class ProductImage
{
    [Required]
    public required string Id { get; set; }
    [Required]
    public required string FileName { get; set; }
    [Required]
    public required string FilePath { get; set; }
}