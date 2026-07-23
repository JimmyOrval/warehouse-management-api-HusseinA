using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Dashboard.Queries;

public class GetDashboardQueryHandler(
    IProductRepository productRepository,
    ISupplierRepository supplierRepository,
    IMapper mapper,
    ILogger<GetDashboardQueryHandler> logger)
    : IRequestHandler<GetDashboardQuery, Result<DashboardViewModel>>
{
    public async Task<Result<DashboardViewModel>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var productCount = await productRepository.GetCountAsync(cancellationToken);
        var supplierCount = await supplierRepository.GetCountAsync(cancellationToken);
        var archivedCount = await productRepository.GetArchivedCountAsync(cancellationToken);

        var dashboard = DashboardBuilder.Build(productCount, supplierCount, archivedCount);
        
        logger.LogInformation("Dashboard retrieved");

        return dashboard;
    }
}