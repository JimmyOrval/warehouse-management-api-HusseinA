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
        var products = await productRepository.GetProductsBySupplierAsync(
                request.SupplierName, request.IsAscending, cancellationToken);
        
        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}