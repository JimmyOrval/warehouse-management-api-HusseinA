using Application.Common;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IMapper mapper,
    IDistributedCache cache,
    ILogger<CreateSupplierCommandHandler> logger)
    : IRequestHandler<CreateSupplierCommand, string>
{
    public async Task<string> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = mapper.Map<Supplier>(request);
        
        supplierRepository.Add(supplier);
        await supplierRepository.SaveChangesAsync(cancellationToken);
        
        await cache.RemoveAsync(SupplierCacheKeys.ById(supplier.Id), cancellationToken);
        await cache.RemoveAsync(SupplierCacheKeys.SuppliersList, cancellationToken);
        
        logger.LogInformation("Supplier {SupplierId} created", supplier.Id);
        
        return supplier.Id;
    }
}