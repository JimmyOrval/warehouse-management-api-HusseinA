namespace Application.ViewModels;

public class WarehouseItemViewModel
{
    public required string Id { get; init; }

    public required string ProductId { get; init; }

    public string? ProductName { get; init; }

    public required string Location { get; init; }

    public int QuantityInStock { get; init; }
}