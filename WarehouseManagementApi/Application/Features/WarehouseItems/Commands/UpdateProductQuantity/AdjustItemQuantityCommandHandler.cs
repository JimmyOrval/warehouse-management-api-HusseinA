using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.UpdateProductQuantity;

public class AdjustItemQuantityCommandHandler(
    IProductRepository productRepository,
    IMapper mapper,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<AdjustItemQuantityCommandHandler> logger)
    : IRequestHandler<AdjustItemQuantityCommand, WarehouseItemViewModel>
{
    public async Task<WarehouseItemViewModel> Handle(AdjustItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            logger.LogWarning("Quantity update failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }

        var item = productRepository.GetWarehouseItem(request.Id);
        if (item == null)
        {
            logger.LogWarning("Quantity update failed: warehouse item {ProductId} not found", request.Id);
            throw new NotFoundException($"No warehouse item found in '{request.Location}'");
        }

        var oldQuantity = item.QuantityInStock;
        
        item.QuantityInStock = request.Quantity;
        item.LastStockUpdate = DateTime.Now;
        
        var currentQuantity = productRepository.GetTotalQuantity(request.Id);
        if (currentQuantity == 0 && currentQuantity < oldQuantity)
        {
            product.SetOutOfStock();
        }

        await productRepository.SaveChangesAsync(cancellationToken);

        if (currentQuantity == 0 && currentQuantity < oldQuantity)
        {
            await cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);
            cacheStats.RecordRemoval(ProductCacheKeys.ById(product.Id));
            
            foreach(var key in ProductCacheKeys.ListVariations)
            {
                await cache.RemoveAsync(key, cancellationToken);
                cacheStats.RecordRemoval(key);
            }
        }
        
        logger.LogInformation(
            "Product {ProductId} quantity updated at {Location} " +
            "from {OldQuantity} to {NewQuantity}",
            request.Id, request.Location, oldQuantity, request.Quantity);
        
        return mapper.Map<WarehouseItemViewModel>(item);
    }
}