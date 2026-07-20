using System.Text.Json;
using Application.Common;
using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public class ListSuppliersQueryHandler(
    ISupplierRepository supplierRepository,
    IMapper mapper,
    IDistributedCache cache,
    ICacheStatsTracker cacheStats,
    ILogger<ListSuppliersQueryHandler> logger)
    : IRequestHandler<ListSuppliersQuery, IEnumerable<SupplierViewModel>>
{
    public async Task<IEnumerable<SupplierViewModel>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = SupplierCacheKeys.SuppliersList;
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached != null)
        {
            logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            cacheStats.RecordHit();
            return JsonSerializer.Deserialize<IEnumerable<SupplierViewModel>>(cached)!;
        }
        
        logger.LogInformation("Cached miss for {CacheKey}, value cached", cacheKey);
        cacheStats.RecordMiss();
        
        var suppliers = await supplierRepository.GetAllAsync(cancellationToken);
        
        var viewModels = mapper.Map<IEnumerable<SupplierViewModel>>(suppliers).ToList();
        
        await cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(viewModels),
                    new DistributedCacheEntryOptions
                        { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                    cancellationToken);
        
        cacheStats.RecordSet(cacheKey);
        logger.LogInformation("All suppliers retrieved");
        
        return viewModels;
    }
}