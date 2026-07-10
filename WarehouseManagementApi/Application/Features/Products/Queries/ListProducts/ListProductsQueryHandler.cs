using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.ListProducts;

public class ListProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<ListProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = request.OnlyAvailable == true
            ? productRepository.GetAvailable()
            : productRepository.GetAll();

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}