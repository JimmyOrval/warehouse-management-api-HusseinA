using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<GetSupplierByIdQuery, Supplier>
{
    public async Task<Supplier> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.Length != 36)
            throw new ArgumentException("Invalid ID format");
        
        var supplier = supplierRepository.GetById(request.Id);
        
        return supplier ?? throw new KeyNotFoundException("Supplier not found");
    }
}