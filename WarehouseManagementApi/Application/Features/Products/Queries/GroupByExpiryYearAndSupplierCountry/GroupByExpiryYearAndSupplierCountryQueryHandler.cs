using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;

public class GroupByExpiryYearAndSupplierCountryQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GroupByExpiryYearAndSupplierCountryQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GroupByExpiryYearAndSupplierCountryQuery request, CancellationToken cancellationToken)
    {
        var products = (await productRepository
                .GroupByExpiryYearAndSupplierCountryAsync(cancellationToken))
            .SelectMany(g => g);

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}