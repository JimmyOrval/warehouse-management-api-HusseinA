using Application.Common;
using Application.ViewModels;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Dashboard.Queries;

public class GetDashboardQueryHandler(
    IProductRepository productRepository, ISupplierRepository supplierRepository)
    : IRequestHandler<GetDashboardQuery, Result<DashboardViewModel>>
{
    public async Task<Result<DashboardViewModel>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var productCount = await productRepository.GetCountAsync(cancellationToken);
        var supplierCount = await supplierRepository.GetCountAsync(cancellationToken);
        var archivedCount = await productRepository.GetArchivedCountAsync(cancellationToken);

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