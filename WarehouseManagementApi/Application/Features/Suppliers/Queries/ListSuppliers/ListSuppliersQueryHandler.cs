using System.Text.Json;
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
    ILogger<ListSuppliersQueryHandler> logger)
    : IRequestHandler<ListSuppliersQuery, IEnumerable<SupplierViewModel>>
{
    public async Task<IEnumerable<SupplierViewModel>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"suppliers:list";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached != null)
        {
            logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<IEnumerable<SupplierViewModel>>(cached)!;
        }
        
        var suppliers = await supplierRepository.GetAllAsync(cancellationToken);
        
        var viewModels = mapper.Map<IEnumerable<SupplierViewModel>>(suppliers).ToList();
        
        await cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(viewModels),
                    new DistributedCacheEntryOptions
                        { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                    cancellationToken);
        
        logger.LogInformation("Cached miss for {CacheKey}, value cached", cacheKey);
        logger.LogInformation("All suppliers retrieved");
        return viewModels;
    }
}