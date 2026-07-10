using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Commands.ArchiveProduct;

public class ArchiveProductCommandHandler(
    IProductRepository productRepository,
    IMapper mapper)
    : IRequestHandler<ArchiveProductCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        // ID should match GUID format
        if (request.Id?.Length != 36)
            throw new ArgumentException("Invalid ID format");

        var product = productRepository.GetById(request.Id);

        if (product == null)
            throw new KeyNotFoundException("Product not found");

        product.Archive();
        productRepository.SaveChanges();
        return mapper.Map<ProductViewModel>(product);
    }
}