using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GetPagedProducts;

public class GetPagedProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<GetPagedProductsQueryHandler> logger)
    : IRequestHandler<GetPagedProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        var pagedProducts = await productRepository.GetPagedProductsAsync(
                request.PageNumber, request.PageSize, cancellationToken);
        
        logger.LogInformation("Paged products retrieved." +
                              "Products: {ProductCount}." +
                              "Page number: {PageNumber}.",
            request.PageSize, request.PageNumber);
        
        return mapper.Map<IEnumerable<ProductViewModel>>(pagedProducts);
    }
}