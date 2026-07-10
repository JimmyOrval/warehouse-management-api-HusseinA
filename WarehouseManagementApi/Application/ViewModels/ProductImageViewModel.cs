namespace Application.ViewModels;

public class ProductImageViewModel
{
    public required string Id { get; init; }
    
    public required string ProductId { get; init; }
    
    public string? ProductName { get; init; }

    public required string FileName { get; init; }

    public required string FilePath { get; init; }
}