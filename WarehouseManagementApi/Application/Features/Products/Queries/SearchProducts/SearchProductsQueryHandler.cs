using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<SearchProductsQueryHandler> logger)
    : IRequestHandler<SearchProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        // BadRequest if both filters are empty
        if(string.IsNullOrWhiteSpace(request.Name) && string.IsNullOrWhiteSpace(request.Supplier))
            throw new ArgumentException("Both filters empty. Please enter at least one.");
        
        var products = await productRepository.SearchAsync(
            request.Name, request.Supplier, cancellationToken);
        
        logger.LogInformation("Searched products retrieved");

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}