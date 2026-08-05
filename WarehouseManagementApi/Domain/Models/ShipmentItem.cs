using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class ShipmentItem
{
    [Key]
    public required string Id { get; init; } = Guid.NewGuid().ToString();

    [Required]
    public required string ShipmentId { get; init; }
    public virtual Shipment? Shipment { get; init; }

    [Required]
    public required string ProductId { get; init; }
    public virtual Product? Product { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public required int Quantity { get; init; }
}
