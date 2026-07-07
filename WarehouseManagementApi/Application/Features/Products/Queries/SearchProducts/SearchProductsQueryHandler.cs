using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<SearchProductsQuery, List<Product>>
{
    public async Task<List<Product>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        // BadRequest if both filters are empty
        if(string.IsNullOrWhiteSpace(request.Name) && string.IsNullOrWhiteSpace(request.Supplier))
            throw new ArgumentException("Both filters empty. Please enter at least one.");

        return productRepository.Search(request.Name, request.Supplier).ToList();
    }
}