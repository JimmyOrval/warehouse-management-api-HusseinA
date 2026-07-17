using System.Text.Json;
using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(
    ISupplierRepository supplierRepository,
    IMapper mapper,
    IDistributedCache cache,
    ILogger<GetSupplierByIdQueryHandler> logger)
    : IRequestHandler<GetSupplierByIdQuery, SupplierViewModel>
{
    public async Task<SupplierViewModel> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"supplier:{request.Id}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached != null)
        {
            logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<SupplierViewModel>(cached)!;
        }
        
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier == null)
        {
            logger.LogInformation("Supplier {SupplierId} not found", request.Id);
            throw new NotFoundException($"Supplier '{request.Id}' not found");
        }
        
        var viewModel = mapper.Map<SupplierViewModel>(supplier);
        
        await cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(viewModel),
                    new DistributedCacheEntryOptions
                        { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                    cancellationToken);
        
        logger.LogInformation("Cache miss for {CacheKey}, value cached", cacheKey);
        logger.LogInformation("Supplier {SupplierId} retrieved", supplier.Id);

        return viewModel;
    }
}