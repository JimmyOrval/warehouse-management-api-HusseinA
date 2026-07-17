using AutoMapper;
using Domain.Exceptions;
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
        if (await productRepository.SkuExistsAsync(request.Sku, cancellationToken))
        {
            throw new BusinessRuleException($"Product SKU '{request.Sku}' already exists");
        }
        
        var product = mapper.Map<Product>(request);
        
        productRepository.Add(product);
        await productRepository.SaveChangesAsync(cancellationToken);
        
        return product.Id;
    }
}