using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetTotalProductQuantity;

public class GetTotalProductQuantityQueryHandler(
    IProductRepository productRepository,
    ILogger<GetTotalProductQuantityQueryHandler> logger)
    : IRequestHandler<GetTotalProductQuantityQuery, int>
{
    public async Task<int> Handle(GetTotalProductQuantityQuery request, CancellationToken cancellationToken)
    {
        if (await productRepository.GetByIdAsync(
                request.ProductId, cancellationToken) == null)
        {
            logger.LogError("Product {ProductId} not found", request.ProductId);
            throw new NotFoundException($"Product {request.ProductId} not found");
        }
        
        var quantity = await productRepository.GetTotalStockQuantityAsync(request.ProductId, cancellationToken);
        return quantity;
    }
}