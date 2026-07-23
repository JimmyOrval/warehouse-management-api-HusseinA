using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.Queries.ListWarehouseItems;

public class ListWarehouseItemsQueryHandler(
    IWarehouseItemRepository warehouseItemRepository,
    IMapper mapper,
    ILogger<ListWarehouseItemsQueryHandler> logger)
    : IRequestHandler<ListWarehouseItemsQuery, List<WarehouseItemViewModel>>
{
    public async Task<List<WarehouseItemViewModel>> Handle(ListWarehouseItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await warehouseItemRepository.GetAllAsync(cancellationToken);
        logger.LogInformation("All items retrieved");
        
        var itemViewModels = mapper.Map<List<WarehouseItemViewModel>>(items);
        return itemViewModels;
    }
}