using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.GetProductCount;

public class GetProductCountQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductCountQuery, int>
{
    public async Task<int> Handle(GetProductCountQuery request, CancellationToken cancellationToken)
    {
        return productRepository.GetCount();
    }
}