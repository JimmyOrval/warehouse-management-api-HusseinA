using System.Text.Json;
using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    IDistributedCache cache,
    ILogger<GetProductByIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, ProductViewModel?>
{
    public async Task<ProductViewModel?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{request.Id}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached != null)
        {
            logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<ProductViewModel>(cached);
        }
        
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            logger.LogWarning("Product retrieval failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }

        var viewModel = mapper.Map<ProductViewModel>(product);

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(viewModel),
            new DistributedCacheEntryOptions
                { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
            cancellationToken);
        
        logger.LogInformation("Cache miss for {CacheKey}, value cached", cacheKey);
        logger.LogInformation("Product {ProductId} retrieved", request.Id);

        return viewModel;
    }
}