using Application.DTOs;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.ListProducts;

public class ListProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<ListProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = request.OnlyAvailable == true
            ? (List<Product>)productRepository.GetAvailable()
            : (List<Product>)productRepository.GetAll();

        return products.Select(p => new ProductDto(
            p.Id, p.Name, p.Sku, p.Description, p.Price, p.SupplierId, p.ExpiryDate,
            p.IsArchived, p.CreatedAt, p.LastUpdatedAt)).ToList();
    }
}