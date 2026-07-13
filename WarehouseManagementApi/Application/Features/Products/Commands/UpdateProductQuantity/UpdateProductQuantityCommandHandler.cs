using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<UpdateProductQuantityCommand, WarehouseItemViewModel>
{
    public async Task<WarehouseItemViewModel> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        // ID should match GUID format
        if (request.Id?.Length != 36)
            throw new ArgumentException("Invalid ID format");

        // quantity cannot be negative
        if (request.Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative");

        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new KeyNotFoundException("Product not found");

        var item = productRepository.GetWarehouseItem(request.Id, request.Location);
        if (item == null)
            throw new KeyNotFoundException($"No warehouse item found in '{request.Location}'");
        
        // update both the quantity and updated date
        item.QuantityInStock = request.Quantity;
        item.LastStockUpdate = DateTime.Now;
        product.LastUpdatedAt = DateTime.Now;

        await productRepository.SaveChangesAsync(cancellationToken);
        
        return mapper.Map<WarehouseItemViewModel>(item);
    }
}