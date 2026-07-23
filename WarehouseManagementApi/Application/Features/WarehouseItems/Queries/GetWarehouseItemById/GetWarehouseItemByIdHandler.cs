using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.Queries.GetWarehouseItemById;

public class GetWarehouseItemByIdHandler(
    IWarehouseItemRepository warehouseItemRepository,
    IMapper mapper,
    ILogger<GetWarehouseItemByIdHandler> logger)
    : IRequestHandler<GetWarehouseItemByIdQuery, WarehouseItemViewModel>
{
    public async Task<WarehouseItemViewModel> Handle(GetWarehouseItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await warehouseItemRepository.GetByIdAsync(
            request.ItemId,
            cancellationToken);

        if (item == null)
        {
            logger.LogWarning("Item {ItemId} not found", request.ItemId);
            throw new NotFoundException($"Item {request.ItemId} not found");
        }
        
        logger.LogInformation("Item {ItemId} retrieved", request.ItemId);
        return mapper.Map<WarehouseItemViewModel>(item);
    }
}