using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetProductWarehouseItems;

public class GetProductWarehouseItemsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<GetProductWarehouseItemsQueryHandler> logger)
    : IRequestHandler<GetProductWarehouseItemsQuery, List<WarehouseItemViewModel>>
{
    public async Task<List<WarehouseItemViewModel>> Handle(GetProductWarehouseItemsQuery request, CancellationToken cancellationToken)
    {
        if (await productRepository.GetByIdAsync(
                request.ProductId, cancellationToken) == null)
        {
            logger.LogWarning("Items retrieval failed. Product {ProductId} not found", request.ProductId);
            throw new NotFoundException($"Items retrieval failed. Product {request.ProductId} not found");
        }
        
        var items = await productRepository.GetProductWarehouseItemsAsync(request.ProductId, cancellationToken);
        logger.LogInformation("Items retrieved for product {ProductId}", request.ProductId);
        return mapper.Map<List<WarehouseItemViewModel>>(items);
    }
}