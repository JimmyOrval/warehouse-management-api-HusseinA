using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Queries.GroupByExpiryYear;

public class GroupByExpiryYearQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GroupByExpiryYearQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GroupByExpiryYearQuery request, CancellationToken cancellationToken)
    {
        var products = (await productRepository
                .GroupByExpiryYearAsync(cancellationToken))
            .SelectMany(g => g);

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}