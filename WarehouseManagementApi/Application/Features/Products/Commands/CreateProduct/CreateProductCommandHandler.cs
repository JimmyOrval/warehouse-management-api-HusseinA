using Domain.Interfaces;
using Domain.Models;
using MediatR;
using WarehouseManagementApi;

namespace Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<CreateProductCommand, string>
{
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // check if duplicate SKU already exists
        if (productRepository.SkuExists(request.Sku))
        {
            throw new Exception($"Product SKU already exists");
        }
        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Sku = request.Sku,
            Description = request.Description,
            Price = request.Price,
            SupplierId = request.SupplierId,
            ExpiryDate = request.ExpiryDate,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };
        
        productRepository.Add(product);
        return product.Id;
    }
}