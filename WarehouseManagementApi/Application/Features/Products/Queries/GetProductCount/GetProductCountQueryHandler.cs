using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetProductCount;

public class GetProductCountQueryHandler(IProductRepository productRepository,
    ILogger<GetProductCountQueryHandler> logger)
    : IRequestHandler<GetProductCountQuery, int>
{
    public async Task<int> Handle(GetProductCountQuery request, CancellationToken cancellationToken)
    {
        var productCount = await productRepository.GetCountAsync(cancellationToken);
        
        logger.LogInformation("Product count retrieved");
        
        return productCount;
    }
}