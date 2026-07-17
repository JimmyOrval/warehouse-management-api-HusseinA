namespace Application.ViewModels;

public class DashboardViewModel
{
    public int ProductCount { get; init; }

    public int SupplierCount { get; init; }

    public int ArchivedProductCount { get; init; }

    public DateTime GeneratedAt { get; init; }
}