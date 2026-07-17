using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<UpdateProductQuantityCommand, WarehouseItemViewModel>
{
    public async Task<WarehouseItemViewModel> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new NotFoundException($"Product '{request.Id}' not found");

        var item = productRepository.GetWarehouseItem(request.Id, request.Location);
        if (item == null)
            throw new NotFoundException($"No warehouse item found in '{request.Location}'");
        
        // update both the quantity and updated date
        item.QuantityInStock = request.Quantity;
        item.LastStockUpdate = DateTime.Now;
        product.LastUpdatedAt = DateTime.Now;

        await productRepository.SaveChangesAsync(cancellationToken);
        
        return mapper.Map<WarehouseItemViewModel>(item);
    }
}