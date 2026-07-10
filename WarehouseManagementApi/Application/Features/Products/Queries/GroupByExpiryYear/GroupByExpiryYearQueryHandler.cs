using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYear;

public class GroupByExpiryYearQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GroupByExpiryYearQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GroupByExpiryYearQuery request, CancellationToken cancellationToken)
    {
        var products = productRepository.GroupByExpiryYear()
            .SelectMany(group => group);
            
        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}