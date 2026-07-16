using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IMapper mapper,
    ILogger<CreateSupplierCommandHandler> logger)
    : IRequestHandler<CreateSupplierCommand, string>
{
    public async Task<string> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = mapper.Map<Supplier>(request);
        
        supplierRepository.Add(supplier);
        await supplierRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Supplier {SupplierId} created", supplier.Id);
        
        return supplier.Id;
    }
}