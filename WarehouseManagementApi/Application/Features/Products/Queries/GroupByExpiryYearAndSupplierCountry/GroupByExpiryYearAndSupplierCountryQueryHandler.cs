using Application.DTOs;
using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;

public class GroupByExpiryYearAndSupplierCountryQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GroupByExpiryYearAndSupplierCountryQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GroupByExpiryYearAndSupplierCountryQuery request, CancellationToken cancellationToken)
    {
        var products = productRepository.GroupByExpiryYearAndSupplierCountry();
        
        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}