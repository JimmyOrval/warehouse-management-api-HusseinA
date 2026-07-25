using Application.Common;
using AutoMapper;
using Domain.Events.Contracts;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IMapper mapper,
    IEventPublisher eventPublisher,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, string>
{
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // check if duplicate SKU already exists
        if (await productRepository.SkuExistsAsync(request.Sku, cancellationToken))
        {
            logger.LogWarning(
                "Product creation failed: SKU {Sku} already exists",
                request.Sku);
            throw new BusinessRuleException($"Product SKU '{request.Sku}' already exists");
        }
        
        var product = mapper.Map<Product>(request);
        
        productRepository.Add(product);
        await productRepository.SaveChangesAsync(cancellationToken);
        
        await cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);
        cacheStats.RecordRemoval(ProductCacheKeys.ById(product.Id));
        
        foreach(var key in ProductCacheKeys.ListVariations)
        {
            await cache.RemoveAsync(key, cancellationToken);
            cacheStats.RecordRemoval(key);
        }
        
        logger.LogInformation("Product {ProductId} created", product.Id);
        
        await eventPublisher.PublishAsync(new ProductCreated()
        {
            CorrelationId = Guid.NewGuid().ToString(),
            EventType = "ProductCreated",
            RelatedEntityId = product.Id,
            RelatedEntityType = "Product",
            Severity = "Info",
            ProductName = product.Name,
            Sku = product.Sku,
            SupplierName = product.Supplier?.Name ?? "Unknown supplier"
        }, "product.created", cancellationToken);
        
        logger.LogInformation("Published ProductCreated for product {ProductId}.",
            product.Id);
        
        return product.Id;
    }
}