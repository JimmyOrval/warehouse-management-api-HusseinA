using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(
    [property: GuidString]
    string Id)
    : IRequest<ProductViewModel?>;