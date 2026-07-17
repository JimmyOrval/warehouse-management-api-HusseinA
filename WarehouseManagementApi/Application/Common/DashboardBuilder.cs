using Application.ViewModels;

namespace Application.Common;

public class DashboardBuilder
{
    // using the generic Result<T> I created
    public static Result<DashboardViewModel> Build(
        int productCount,
        int supplierCount,
        int archivedCount)
    {
        var dashboard = new DashboardViewModel
        {
            ProductCount = productCount,
            SupplierCount = supplierCount,
            ArchivedProductCount = archivedCount,
            GeneratedAt = DateTime.UtcNow
        };

        return Result<DashboardViewModel>.Success(dashboard);
    }
}