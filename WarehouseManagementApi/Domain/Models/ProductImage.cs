using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class ProductImage
{
    [Required]
    public required string Id { get; init; } = Guid.NewGuid().ToString();
    [Required]
    public required string ProductId { get; init; }
    public virtual Product? Product { get; init; }
    [Required]
    public required string FileName { get; init; }
    [Required]
    public required string FilePath { get; init; }
}