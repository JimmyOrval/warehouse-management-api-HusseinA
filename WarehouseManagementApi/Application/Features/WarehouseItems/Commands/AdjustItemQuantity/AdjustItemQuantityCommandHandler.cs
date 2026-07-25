using Application.ViewModels;
using AutoMapper;
using Domain.Events.Contracts;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.Commands.AdjustItemQuantity;

public class AdjustItemQuantityCommandHandler(
    IWarehouseItemRepository warehouseItemRepository,
    IEventPublisher eventPublisher,
    IConfiguration configuration,
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

        var previousQuantity = item.QuantityInStock;
        var minimumLowQuantity = configuration.GetValue<int>("Notifications:MinimumLowQuantity");
        
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
            request.ItemId, previousQuantity, request.Quantity);

        await eventPublisher.PublishAsync(new StockAdjusted
        {
            CorrelationId = Guid.NewGuid().ToString(),
            EventType = "StockAdjusted",
            RelatedEntityId = item.Id,
            RelatedEntityType = "WarehouseItem",
            Severity = "Info",
            ProductName = item.Product?.Name ?? "Unknown product",
            PreviousQuantity = previousQuantity,
            NewQuantity = item.QuantityInStock
        }, "stock.adjusted", cancellationToken);
        
        logger.LogInformation("Published StockAdjusted for item {ItemId}. " +
                              "Old quantity: {OldQuantity}. New Quantity: {NewQuantity}",
            item.Id, previousQuantity, item.QuantityInStock);

        if(previousQuantity >= minimumLowQuantity && item.QuantityInStock < minimumLowQuantity)
        {
            await eventPublisher.PublishAsync(new StockLowDetected
            {
                CorrelationId = Guid.NewGuid().ToString(),
                EventType = "StockLowDetected",
                RelatedEntityId = item.Id,
                RelatedEntityType = "WarehouseItem",
                Severity = "Warning",
                ProductName = item.Product?.Name ?? "Unknown product",
                CurrentQuantity = previousQuantity,
                MinimumQuantity = item.QuantityInStock
            }, "stock.low", cancellationToken);
        
            logger.LogInformation("Published StockLowDetected for item {ItemId}. " +
                                  "Quantity {OldQuantity} below minimum {MinimumLowQuantity}",
                item.Id, item.QuantityInStock, minimumLowQuantity);
        }
        
        return mapper.Map<WarehouseItemViewModel>(item);
    }
}