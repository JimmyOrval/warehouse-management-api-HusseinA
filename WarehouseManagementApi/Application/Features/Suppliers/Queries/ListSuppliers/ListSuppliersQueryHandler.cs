using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public class ListSuppliersQueryHandler(
    ISupplierRepository supplierRepository,
    IMapper mapper,
    ILogger<ListSuppliersQueryHandler> logger)
    : IRequestHandler<ListSuppliersQuery, IEnumerable<SupplierViewModel>>
{
    public async Task<IEnumerable<SupplierViewModel>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await supplierRepository.GetAllAsync(cancellationToken);
        
        logger.LogInformation("All suppliers retrieved");
        
        return mapper.Map<IEnumerable<SupplierViewModel>>(suppliers);
    }
}