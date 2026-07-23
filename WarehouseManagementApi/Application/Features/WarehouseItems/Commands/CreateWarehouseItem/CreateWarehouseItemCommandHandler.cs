using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.Commands.CreateWarehouseItem;

public class CreateWarehouseItemCommandHandler(
    IWarehouseItemRepository warehouseItemRepository,
    IMapper mapper,
    ILogger<CreateWarehouseItemCommandHandler> logger)
    : IRequestHandler<CreateWarehouseItemCommand, string>
{
    public async Task<string> Handle(CreateWarehouseItemCommand request, CancellationToken cancellationToken)
    {
        if (await warehouseItemRepository.LocationExistsAsync(request.Location, cancellationToken))
        {
            logger.LogWarning(
                "Item creation failed: Location {Location} already exists",
                request.Location);
            throw new BusinessRuleException($"Item location {request.Location} already exists");
        }

        var item = mapper.Map<WarehouseItem>(request);
        
        warehouseItemRepository.Add(item);
        await warehouseItemRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Item {ItemId} created", item.Id);
        return item.Id;
    }
}