using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetProductByIdQuery, ProductViewModel?>
{
    public async Task<ProductViewModel?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            logger.LogWarning("Product retrieval failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }
        
        logger.LogInformation("Product {ProductId} retrieved", request.Id);

        return mapper.Map<ProductViewModel>(product);
    }
}