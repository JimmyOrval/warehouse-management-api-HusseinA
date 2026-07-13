using Application.DTOs;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<SearchProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        // BadRequest if both filters are empty
        if(string.IsNullOrWhiteSpace(request.Name) && string.IsNullOrWhiteSpace(request.Supplier))
            throw new ArgumentException("Both filters empty. Please enter at least one.");

        return productRepository.Search(request.Name, request.Supplier).Select(p => new ProductDto(
            p.Id, p.Name, p.Sku, p.Description, p.Price, p.SupplierId, p.ExpiryDate,
            p.IsArchived, p.CreatedAt, p.LastUpdatedAt)).ToList();
    }
}