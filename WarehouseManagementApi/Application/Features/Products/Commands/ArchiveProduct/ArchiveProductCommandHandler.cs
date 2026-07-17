using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
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
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new NotFoundException($"Product '{request.Id}' not found");

        product.Archive();
        await productRepository.SaveChangesAsync(cancellationToken);
        return mapper.Map<ProductViewModel>(product);
    }
}