using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductCommandHandler(
    IProductRepository productRepository,
    ISupplierRepository supplierRepository,
    IMapper mapper,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<AssignSupplierToProductCommandHandler> logger)
    : IRequestHandler<AssignSupplierToProductCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            logger.LogWarning("Product assignment failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }
        
        if(product.Status == ProductStatus.Archived)
        {
            logger.LogWarning("Product assignment failed: product {ProductId} is unavailable", request.Id);
            throw new BusinessRuleException($"Product '{product.Name}' is unavailable");
        }

        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);

        if (supplier == null)
        {
            logger.LogWarning("Product assignment failed: supplier {SupplierId} not found", request.SupplierId);
            throw new NotFoundException($"Supplier '{request.SupplierId}' not found");
        }
        
        product.AssignSupplier(supplier);
        await productRepository.SaveChangesAsync(cancellationToken);
        
        await cache.RemoveAsync(SupplierCacheKeys.ById(supplier.Id), cancellationToken);
        cacheStats.RecordRemoval(SupplierCacheKeys.ById(supplier.Id));
        await cache.RemoveAsync(SupplierCacheKeys.SuppliersList, cancellationToken);
        cacheStats.RecordRemoval(SupplierCacheKeys.SuppliersList);
        
        logger.LogInformation("Product {ProductId} assigned to {SupplierId}", request.Id, request.SupplierId);
        
        return mapper.Map<ProductViewModel>(product);
    }
}