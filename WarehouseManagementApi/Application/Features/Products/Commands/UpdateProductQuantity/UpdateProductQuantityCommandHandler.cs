using Application.ViewModels;
using AutoMapper;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<UpdateProductQuantityCommandHandler> logger)
    : IRequestHandler<UpdateProductQuantityCommand, WarehouseItemViewModel>
{
    public async Task<WarehouseItemViewModel> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            logger.LogWarning("Quantity update failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }

        var item = productRepository.GetWarehouseItem(request.Id, request.Location);
        if (item == null)
        {
            logger.LogWarning("Quantity update failed: warehouse item {ProductId} not found", request.Id);
            throw new NotFoundException($"No warehouse item found in '{request.Location}'");
        }

        var oldQuantity = item.QuantityInStock;
        
        item.QuantityInStock = request.Quantity;
        item.LastStockUpdate = DateTime.Now;
        
        var currentQuantity = productRepository.GetQuantity(request.Id);
        if (currentQuantity == 0)
            product.SetOutOfStock();

        await productRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Product {ProductId} quantity updated at {Location} " +
            "from {OldQuantity} to {NewQuantity}",
            request.Id, request.Location, oldQuantity, request.Quantity);
        
        return mapper.Map<WarehouseItemViewModel>(item);
    }
}