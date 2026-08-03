using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.ExpiringSoonProducts;

public class ExpiringSoonProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<ExpiringSoonProductsQueryHandler> logger)
    : IRequestHandler<ExpiringSoonProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(
        ExpiringSoonProductsQuery request, CancellationToken cancellationToken)
    {
        // the generated code gave DateTime.Now, so I changed it
        var now = DateTime.UtcNow;
        var cutoff = now.AddDays(30);
 
        var products = await productRepository.GetExpiringSoonAsync(now, cutoff, cancellationToken);
 
        logger.LogInformation("Retrieved {Count} products expiring within 30 days", products.Count);
 
        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}