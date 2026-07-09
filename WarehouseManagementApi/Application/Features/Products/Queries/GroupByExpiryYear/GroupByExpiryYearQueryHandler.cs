using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYear;

public class GroupByExpiryYearQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GroupByExpiryYearQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GroupByExpiryYearQuery request, CancellationToken cancellationToken)
    {
        return productRepository.GroupByExpiryYear()
            .SelectMany(group => group)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Sku, p.Description, p.Price, p.SupplierId, p.ExpiryDate,
                p.IsArchived, p.CreatedAt, p.LastUpdatedAt)).ToList();
    }
}