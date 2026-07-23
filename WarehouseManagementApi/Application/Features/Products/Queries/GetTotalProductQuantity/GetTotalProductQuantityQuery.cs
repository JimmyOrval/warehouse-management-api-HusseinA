using MediatR;

namespace Application.Features.Products.Queries.GetTotalProductQuantity;

public record GetTotalProductQuantityQuery(string ProductId) : IRequest<int>;