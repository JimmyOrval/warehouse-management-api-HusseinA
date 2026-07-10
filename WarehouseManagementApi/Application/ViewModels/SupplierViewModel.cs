namespace Application.ViewModels;

public class SupplierViewModel
{
    public required string Id { get; init; }

    public required string Name { get; init; } = null!;

    public required string Country { get; init; } = null!;

    public required string ContactEmail { get; init; } = null!;

    public required string Phone { get; init; } = null!;

    public bool IsActive { get; init; }
}