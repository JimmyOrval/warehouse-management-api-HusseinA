using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<CreateProductCommand, string>
{
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // check if duplicate SKU already exists
        if (productRepository.SkuExists(request.Sku))
        {
            throw new Exception($"Product SKU already exists");
        }
        
        var product = mapper.Map<Product>(request);
        
        productRepository.Add(product);
        return product.Id;
    }
}