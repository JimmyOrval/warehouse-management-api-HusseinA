using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(string Id) : IRequest<ProductViewModel?>;