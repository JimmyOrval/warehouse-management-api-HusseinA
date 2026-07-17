using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductPrice;

public record UpdateProductPriceCommand(
    [property: GuidString]
    string Id,
    decimal NewPrice)
    : IRequest<ProductViewModel>;