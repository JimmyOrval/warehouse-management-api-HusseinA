using Application.DTOs;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<GetSupplierByIdQuery, SupplierDto>
{
    public async Task<SupplierDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.Length != 36)
            throw new ArgumentException("Invalid ID format");
        
        var supplier = supplierRepository.GetById(request.Id);
        
        if(supplier == null)
            throw new KeyNotFoundException("Supplier not found");
        
        return new SupplierDto(
            supplier.Id, supplier.Name, supplier.Country,
            supplier.ContactEmail, supplier.Phone, supplier.IsActive);
    }
}