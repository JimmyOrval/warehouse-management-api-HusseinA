using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;

public class GroupByExpiryYearAndSupplierCountryQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GroupByExpiryYearAndSupplierCountryQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GroupByExpiryYearAndSupplierCountryQuery request, CancellationToken cancellationToken)
    {
        return productRepository.GroupByExpiryYearAndSupplierCountry()
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Sku,
                p.Description, p.Price,
                p.SupplierId, p.ExpiryDate,
                p.IsArchived,p.CreatedAt,
                p.LastUpdatedAt
            ));
    }
}