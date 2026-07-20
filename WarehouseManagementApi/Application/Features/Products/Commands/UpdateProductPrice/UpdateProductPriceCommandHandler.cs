using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.UpdateProductPrice;

public class UpdateProductPriceCommandHandler(
    IProductRepository productRepository,
    IMapper mapper,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<UpdateProductPriceCommandHandler> logger)
    : IRequestHandler<UpdateProductPriceCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            logger.LogWarning("Price update failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }
        
        var oldPrice = product.Price;
        product.ChangePrice(request.NewPrice);
        
        await productRepository.SaveChangesAsync(cancellationToken);
        
        await cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);
        cacheStats.RecordRemoval(ProductCacheKeys.ById(product.Id));
        
        foreach(var key in ProductCacheKeys.ListVariations)
        {
            await cache.RemoveAsync(key, cancellationToken);
            cacheStats.RecordRemoval(key);
        }
        
        logger.LogInformation(
            "Product {ProductId} price updated from {OldPrice} to {NewPrice}",
            product.Id, oldPrice, request.NewPrice);
        
        return mapper.Map<ProductViewModel>(product);
    }
}