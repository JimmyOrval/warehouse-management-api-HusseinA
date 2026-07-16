using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.ArchiveProduct;

public class ArchiveProductCommandHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<ArchiveProductCommandHandler> logger)
    : IRequestHandler<ArchiveProductCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            logger.LogWarning("Archiving failed: product {ProductId} not found", request.Id);
            throw new NotFoundException($"Product '{request.Id}' not found");
        }

        product.Archive();
        await productRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Archived product {ProductId}", request.Id);
        
        return mapper.Map<ProductViewModel>(product);
    }
}