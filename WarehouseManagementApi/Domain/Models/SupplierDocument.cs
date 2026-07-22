using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class SupplierDocument
{
    [Required]
    public required string Id { get; init; } = Guid.NewGuid().ToString();
    [Required]
    public required string SupplierId { get; init; }
    public virtual Supplier? Supplier { get; init; }
    [Required]
    public required string FileName { get; init; }
    [Required]
    public required string ObjectKey { get; init; }
    [Required]
    public required string ContentType { get; init; }
    public required long Size { get; init; }
    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;
}