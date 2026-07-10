using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GetPagedProducts;

public class GetPagedProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GetPagedProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        var pagedProducts = productRepository.GetPagedProducts(
                request.PageNumber, request.PageSize);
        
        return mapper.Map<IEnumerable<ProductViewModel>>(pagedProducts);
    }
}