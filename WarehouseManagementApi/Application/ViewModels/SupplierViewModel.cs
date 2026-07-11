namespace Application.ViewModels;

public class SupplierViewModel
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Country { get; init; }

    public required string ContactEmail { get; init; }

    public required string Phone { get; init; }

    public bool IsActive { get; init; }
}