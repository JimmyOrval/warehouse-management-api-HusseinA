using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
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
        var products = await productRepository.SearchAsync(
            request.Name, request.Supplier, cancellationToken);
        
        logger.LogInformation("Searched products retrieved");

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}