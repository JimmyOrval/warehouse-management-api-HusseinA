using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<CreateSupplierCommand, string>
{
    public async Task<string> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name =  request.Name,
            Country =  request.Country,
            ContactEmail = request.ContactEmail,
            Phone =  request.Phone,
            IsActive = true
        };
        
        supplierRepository.Add(supplier);
        return supplier.Id;
    }
}