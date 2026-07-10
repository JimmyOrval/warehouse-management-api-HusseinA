using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GetProductsBySupplier;

public class GetProductsBySupplierQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GetProductsBySupplierQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GetProductsBySupplierQuery request, CancellationToken cancellationToken)
    {
        var products = productRepository.GetProductsBySupplier(
                request.SupplierName, request.IsAscending);
        
        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}