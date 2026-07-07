using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.ListProducts;

public class ListProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<ListProductsQuery, IEnumerable<Product>>
{
    public async Task<IEnumerable<Product>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        return request.OnlyAvailable == true ? productRepository.GetAvailable() : productRepository.GetAll();
    }
}