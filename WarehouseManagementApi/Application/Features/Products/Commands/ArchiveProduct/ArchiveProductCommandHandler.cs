using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.ArchiveProduct;

public class ArchiveProductCommandHandler(
    IProductRepository productRepository,
    IMapper mapper,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<ArchiveProductCommandHandler> logger)
    : IRequestHandler<ArchiveProductCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            logger.LogWarning("Archiving failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }

        product.Archive();
        await productRepository.SaveChangesAsync(cancellationToken);
        
        await cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);
        cacheStats.RecordRemoval(ProductCacheKeys.ById(product.Id));
        
        foreach(var key in ProductCacheKeys.ListVariations)
        {
            await cache.RemoveAsync(key, cancellationToken);
            cacheStats.RecordRemoval(key);
        }
        
        logger.LogInformation("Archived product {ProductId}", request.Id);
        
        return mapper.Map<ProductViewModel>(product);
    }
}