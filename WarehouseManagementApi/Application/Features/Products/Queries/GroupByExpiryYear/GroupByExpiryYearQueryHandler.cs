using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries.GroupByExpiryYear;

public class GroupByExpiryYearQueryHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<GroupByExpiryYearQueryHandler> logger)
    : IRequestHandler<GroupByExpiryYearQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(GroupByExpiryYearQuery request, CancellationToken cancellationToken)
    {
        var products = (await productRepository
                .GroupByExpiryYearAsync(cancellationToken))
            .SelectMany(g => g);
        
        logger.LogInformation("Grouped products retrieved");

        return mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}