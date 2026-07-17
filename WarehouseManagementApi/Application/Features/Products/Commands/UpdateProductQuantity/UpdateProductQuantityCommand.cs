using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductQuantity;

public record UpdateProductQuantityCommand(
    [property: GuidString]
    string Id,
    [property: Quantity]
    int Quantity,
    string Location)
    : IRequest<WarehouseItemViewModel>;