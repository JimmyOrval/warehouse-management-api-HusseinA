using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public class DeactivateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IMapper mapper,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<DeactivateSupplierCommandHandler> logger)
    : IRequestHandler<DeactivateSupplierCommand, SupplierViewModel>
{
    public async Task<SupplierViewModel> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if(supplier == null)
        {
            logger.LogWarning("Supplier deactivation failed: supplier {SupplierId} not found", request.Id);
            throw new NotFoundException($"Supplier '{request.Id}' not found");
        }
        
        supplier.Deactivate();
        await supplierRepository.SaveChangesAsync(cancellationToken);
        
        await cache.RemoveAsync(SupplierCacheKeys.ById(supplier.Id), cancellationToken);
        cacheStats.RecordRemoval(SupplierCacheKeys.ById(supplier.Id));
        await cache.RemoveAsync(SupplierCacheKeys.SuppliersList, cancellationToken);
        cacheStats.RecordRemoval(SupplierCacheKeys.SuppliersList);
        
        logger.LogInformation("Supplier {SupplierId} deactivated", supplier.Id);
        
        return mapper.Map<SupplierViewModel>(supplier);
    }
}