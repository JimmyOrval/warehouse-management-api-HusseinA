using Application.ViewModels;
using AutoMapper;
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
        
        var product = productRepository.GetById(request.Id) != null
            ? productRepository.GetById(request.Id) : null;
        
        return product == null ? throw new KeyNotFoundException("Product not found")
            : mapper.Map<ProductViewModel>(product);
    }
}