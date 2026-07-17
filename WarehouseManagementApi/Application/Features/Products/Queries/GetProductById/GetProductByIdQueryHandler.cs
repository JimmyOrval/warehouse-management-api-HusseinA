using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GetProductByIdQuery, ProductViewModel?>
{
    public async Task<ProductViewModel?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        return product == null
            ? throw new NotFoundException($"Product '{request.Id}' not found")
            : mapper.Map<ProductViewModel>(product);
    }
}