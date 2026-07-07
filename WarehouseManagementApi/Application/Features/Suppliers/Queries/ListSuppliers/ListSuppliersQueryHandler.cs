using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public class ListSuppliersQueryHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<ListSuppliersQuery, IEnumerable<Supplier>>
{
    public async Task<IEnumerable<Supplier>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        return supplierRepository.GetAll();
    }
}