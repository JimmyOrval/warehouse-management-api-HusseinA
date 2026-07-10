using MediatR;

namespace Application.Features.Products.Queries.GetProductCount;

public record GetProductCountQuery() : IRequest<int>;