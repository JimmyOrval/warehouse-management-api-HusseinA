using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductPrice;

public record UpdateProductPriceCommand(string Id, decimal NewPrice) : IRequest<ProductViewModel>;