using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<CreateSupplierCommand, string>
{
    public async Task<string> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = mapper.Map<Supplier>(request);
        
        supplierRepository.Add(supplier);
        supplierRepository.SaveChanges();
        
        return supplier.Id;
    }
}