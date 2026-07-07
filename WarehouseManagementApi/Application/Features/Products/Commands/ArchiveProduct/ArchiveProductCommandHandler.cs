using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.ArchiveProduct;

public class ArchiveProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<ArchiveProductCommand, Product>
{
    public async Task<Product> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        // ID should match GUID format
        if (request.Id?.Length != 36)
            throw new ArgumentException("Invalid ID format");

        var product = productRepository.GetById(request.Id);

        if (product == null)
            throw new KeyNotFoundException("Product not found");

        product.IsArchived = true;
        return product;
    }
}