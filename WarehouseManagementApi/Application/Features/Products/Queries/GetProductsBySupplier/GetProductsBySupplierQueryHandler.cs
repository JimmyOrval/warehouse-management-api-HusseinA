using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GetProductsBySupplier;

public class GetProductsBySupplierQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsBySupplierQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsBySupplierQuery request, CancellationToken cancellationToken)
    {
        return productRepository.GetProductsBySupplier(
                request.SupplierName, request.IsAscending)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Sku, p.Description, p.Price, p.SupplierId, p.ExpiryDate,
                p.IsArchived, p.CreatedAt, p.LastUpdatedAt)).ToList();
    }
}