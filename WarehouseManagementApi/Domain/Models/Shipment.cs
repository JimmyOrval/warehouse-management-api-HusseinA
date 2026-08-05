using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Models;

public class Shipment
{
    [Key]
    [Required]
    public required string Id { get; init; } = Guid.NewGuid().ToString();

    [Required(ErrorMessage = "Supplier ID is required")]
    public required string SupplierId { get; init; }

    public virtual Supplier? Supplier { get; init; }

    // set is private since only domain logic can change it
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Pending;

    [Required]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    private readonly List<ShipmentItem> _items = [];
    public ICollection<ShipmentItem> Items => _items;

    public void AssignProduct(string productId, int quantity)
    {
        if (Status != ShipmentStatus.Pending)
            throw new BusinessRuleException("Cannot assign products once a shipment has left Pending status");

        if (quantity <= 0)
            throw new BusinessRuleException("Quantity must be greater than 0");

        _items.Add(new ShipmentItem
        {
            Id = Guid.NewGuid().ToString(),
            ShipmentId = Id,
            ProductId = productId,
            Quantity = quantity
        });

        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ShipmentStatus newStatus)
    {
        if (Status is ShipmentStatus.Delivered or ShipmentStatus.Cancelled)
            throw new BusinessRuleException($"Shipment is already {Status} and cannot be changed further");

        Status = newStatus;
        LastUpdatedAt = DateTime.UtcNow;
    }
}
