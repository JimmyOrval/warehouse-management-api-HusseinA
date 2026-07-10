using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public class ListSuppliersQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<ListSuppliersQuery, IEnumerable<SupplierViewModel>>
{
    public async Task<IEnumerable<SupplierViewModel>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = supplierRepository.GetAll();
        
        return mapper.Map<IEnumerable<SupplierViewModel>>(suppliers);
    }
}