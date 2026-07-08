using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public class ListSuppliersQueryHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<ListSuppliersQuery, IEnumerable<SupplierDto>>
{
    public async Task<IEnumerable<SupplierDto>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        return supplierRepository.GetAll().Select(
            s => new SupplierDto(
                s.Id, s.Name, s.Country,
                s.ContactEmail, s.Phone, s.IsActive))
            .ToList();
    }
}