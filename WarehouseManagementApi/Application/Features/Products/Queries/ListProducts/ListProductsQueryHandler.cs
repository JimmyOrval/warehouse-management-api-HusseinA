using System.Text.Json;
using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.ListProducts;

public class ListProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<ListProductsQueryHandler> logger)
    : IRequestHandler<ListProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"products:list:{request.OnlyAvailable}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached != null)
        {
            logger.LogInformation("Cached hit for {cacheKey}", cacheKey);
            cacheStats.RecordHit();
            return JsonSerializer.Deserialize<IEnumerable<ProductViewModel>>(cached)!;
        }
        
        IEnumerable<Product> products;
        if(request.OnlyAvailable == true)
        {
            products = await productRepository.GetAvailableAsync(cancellationToken);
            logger.LogInformation("Available products retrieved");
        }
        else
        {
            products = await productRepository.GetAllAsync(cancellationToken);
            logger.LogInformation("All products retrieved");
        }

        var viewModels = mapper.Map<IEnumerable<ProductViewModel>>(products).ToList();

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(viewModels),
            new DistributedCacheEntryOptions
                { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
            cancellationToken);
        
        logger.LogInformation("Cache miss for {cacheKey}, value cached", cacheKey);
        cacheStats.RecordMiss();
        cacheStats.RecordSet(cacheKey);
        return viewModels;
    }
}