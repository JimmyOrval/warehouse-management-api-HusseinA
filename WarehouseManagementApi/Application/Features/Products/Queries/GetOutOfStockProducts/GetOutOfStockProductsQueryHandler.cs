using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetOutOfStockProducts;

public class GetOutOfStockProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<GetOutOfStockProductsQueryHandler> logger)
    : IRequestHandler<GetOutOfStockProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(
        GetOutOfStockProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetOutOfStockAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} out-of-stock products", products.Count);

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}
