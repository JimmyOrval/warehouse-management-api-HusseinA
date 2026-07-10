namespace Application.ViewModels;

public class ProductViewModel
{
    public required string Id { get; init; }

    public required string Name { get; init; }
    
    public required string Sku { get; init; }

    public required string Description { get; init; }

    public decimal Price { get; init; }

    public DateTime ExpiryDate { get; init; }

    public bool IsArchived { get; init; }

    public required string SupplierId { get; init; }

    public string? SupplierName { get; init; }

    public DateTime CreatedAt { get; init; }
}