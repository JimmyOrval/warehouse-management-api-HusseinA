using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetProductsBySupplier;

public class GetProductsBySupplierQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<GetProductsBySupplierQueryHandler> logger)
    : IRequestHandler<GetProductsBySupplierQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GetProductsBySupplierQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsBySupplierAsync(
                request.SupplierName, request.IsAscending, cancellationToken);

        if (products.Count == 0)
        {
            logger.LogWarning("No products found for supplier {SupplierName}", request.SupplierName);
            throw new BusinessRuleException("No products found for supplier");
        }
        
        logger.LogInformation("Product list for supplier {SupplierName} retrieved",
            request.SupplierName);
        
        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}