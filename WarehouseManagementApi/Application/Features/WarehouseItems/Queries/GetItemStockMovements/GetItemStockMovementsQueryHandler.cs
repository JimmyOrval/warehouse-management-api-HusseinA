using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.Queries.GetItemStockMovements;

public class GetItemStockMovementsQueryHandler(
    IWarehouseItemRepository warehouseItemRepository,
    IMapper mapper,
    ILogger<GetItemStockMovementsQueryHandler> logger)
    : IRequestHandler<GetItemStockMovementsQuery, List<StockMovementViewModel>>
{
    public async Task<List<StockMovementViewModel>> Handle(GetItemStockMovementsQuery request, CancellationToken cancellationToken)
    {
        if (await warehouseItemRepository.GetByIdAsync(request.ItemId, cancellationToken) == null)
        {
            logger.LogWarning("Stock movements retrieval failed. Item {ItemId} not found", request.ItemId);
            throw new NotFoundException($"Stock movements retrieval failed. Item {request.ItemId} not found");
        }
        
        var movements = await warehouseItemRepository.GetStockMovementsAsync(request.ItemId, cancellationToken);
        logger.LogInformation("Stock movements for item {ItemId} retrieved", request.ItemId);
        
        return mapper.Map<List<StockMovementViewModel>>(movements);
    }
}