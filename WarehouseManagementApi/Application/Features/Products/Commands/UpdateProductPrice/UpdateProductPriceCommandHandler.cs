using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductPrice;

public class UpdateProductPriceCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductPriceCommand, Product>
{
    public async Task<Product> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        // ID should match GUID format
        if (request.Id?.Length != 36)
            throw new ArgumentException("Invalid ID format");

        var product = productRepository.GetById(request.Id);
        
        if (product == null)
            throw new KeyNotFoundException("Product not found");
        
        // keep track of old values
        var oldPrice = product.Price;
        var oldLastUpdatedAt = product.LastUpdatedAt;

        // update new values
        product.ChangePrice(request.NewPrice);
        
        // log changes
        Console.WriteLine("Old price: " + oldPrice + ", Old LastUpdatedAt: " + oldLastUpdatedAt +
                          ", New Price: " + product.Price + ", New LastUpdatedAt: " + product.LastUpdatedAt);
        
        return product;
    }
}