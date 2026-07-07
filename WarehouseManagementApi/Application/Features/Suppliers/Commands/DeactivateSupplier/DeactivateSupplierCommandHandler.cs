using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public class DeactivateSupplierCommandHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<DeactivateSupplierCommand, Supplier>
{
    public async Task<Supplier> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        if (request.Id.Length != 36)
            throw new ArgumentException("Invalid ID format");
        
        var supplier = supplierRepository.GetById(request.Id);
        
        if(supplier == null)
            throw new KeyNotFoundException("Supplier not found");
        
        supplier.IsActive = false;
        return supplier;
    }
}