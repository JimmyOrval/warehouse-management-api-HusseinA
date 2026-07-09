using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GetPagedProducts;

public class GetPagedProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetPagedProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        return productRepository.GetPagedProducts(
                request.PageNumber, request.PageSize)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Sku, p.Description, p.Price, p.SupplierId, p.ExpiryDate,
                p.IsArchived, p.CreatedAt, p.LastUpdatedAt));
    }
}