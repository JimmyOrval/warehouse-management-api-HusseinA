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
        if (request.Id.Length != 36)
            throw new ArgumentException("Invalid ID format");
        
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        return product == null
            ? throw new NotFoundException($"Product '{request.Id}' not found")
            : mapper.Map<ProductViewModel>(product);
    }
}