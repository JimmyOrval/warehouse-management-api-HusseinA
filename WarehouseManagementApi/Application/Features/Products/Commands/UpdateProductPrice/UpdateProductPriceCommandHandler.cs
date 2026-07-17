using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductPrice;

public class UpdateProductPriceCommandHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<UpdateProductPriceCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
            throw new NotFoundException($"Product '{request.Id}' not found");
        
        // keep track of old values
        var oldPrice = product.Price;
        var oldLastUpdatedAt = product.LastUpdatedAt;

        // update new values
        product.ChangePrice(request.NewPrice);
        
        // log changes
        Console.WriteLine("Old price: " + oldPrice + ", Old LastUpdatedAt: " + oldLastUpdatedAt +
                          ", New Price: " + product.Price + ", New LastUpdatedAt: " + product.LastUpdatedAt);
        
        await productRepository.SaveChangesAsync(cancellationToken);
        return mapper.Map<ProductViewModel>(product);
    }
}