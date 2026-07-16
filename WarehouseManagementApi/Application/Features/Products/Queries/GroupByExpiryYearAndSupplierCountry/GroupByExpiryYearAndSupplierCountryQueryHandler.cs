using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;

public class GroupByExpiryYearAndSupplierCountryQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<GroupByExpiryYearAndSupplierCountryQueryHandler> logger)
    : IRequestHandler<GroupByExpiryYearAndSupplierCountryQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GroupByExpiryYearAndSupplierCountryQuery request, CancellationToken cancellationToken)
    {
        var products = (await productRepository
                .GroupByExpiryYearAndSupplierCountryAsync(cancellationToken))
            .SelectMany(g => g);
        
        logger.LogInformation("Grouped products retrieved");

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}