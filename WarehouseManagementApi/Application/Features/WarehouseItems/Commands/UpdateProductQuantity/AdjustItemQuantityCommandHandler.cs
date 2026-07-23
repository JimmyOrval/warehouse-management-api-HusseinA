using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.Commands.UpdateProductQuantity;

public class AdjustItemQuantityCommandHandler(
    IWarehouseItemRepository warehouseItemRepository,
    IMapper mapper,
    ILogger<AdjustItemQuantityCommandHandler> logger)
    : IRequestHandler<AdjustItemQuantityCommand, WarehouseItemViewModel>
{
    public async Task<WarehouseItemViewModel> Handle(AdjustItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var item = await warehouseItemRepository.GetByIdAsync(
            request.ItemId,
            cancellationToken);
        
        if (item == null)
        {
            logger.LogWarning("Quantity update failed: warehouse item {ItemId} not found", request.ItemId);
            throw new NotFoundException($"Warehouse item {request.ItemId} not found");
        }

        var oldQuantity = item.QuantityInStock;
        
        switch (request.Quantity)
        {
            case > 0:
                item.StockIn(request.Quantity);
                break;
            case < 0:
                item.StockOut(Math.Abs(request.Quantity));
                break;
            default:
                throw new ValidationException("Invalid quantity");
        }

        await warehouseItemRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Item {ItemId} quantity updated " +
            "from {OldQuantity} to {NewQuantity}",
            request.ItemId, oldQuantity, request.Quantity);
        
        return mapper.Map<WarehouseItemViewModel>(item);
    }
}