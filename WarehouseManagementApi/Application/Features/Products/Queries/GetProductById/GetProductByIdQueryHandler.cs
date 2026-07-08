using Application.DTOs;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.Length != 36)
            throw new ArgumentException("Invalid ID format");
        
        var product = productRepository.GetById(request.Id) != null
            ? productRepository.GetById(request.Id) : null;
        
        if(product == null)
            throw new KeyNotFoundException("Product not found");
        
        return new ProductDto(
            product.Id, product.Name,  product.Sku, product.Description, product.Price,
            product.SupplierId, product.ExpiryDate, product.IsArchived,
            product.CreatedAt, product.LastUpdatedAt);
    }
}