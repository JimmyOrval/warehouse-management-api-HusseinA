using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.ListProducts;

public class ListProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<ListProductsQueryHandler> logger)
    : IRequestHandler<ListProductsQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Product> products;
        if(request.OnlyAvailable == true)
        {
            products = await productRepository.GetAvailableAsync(cancellationToken);
            logger.LogInformation("Available products retrieved");
        }
        else
        {
            products = await productRepository.GetAllAsync(cancellationToken);
            logger.LogInformation("All products retrieved");
        }

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}