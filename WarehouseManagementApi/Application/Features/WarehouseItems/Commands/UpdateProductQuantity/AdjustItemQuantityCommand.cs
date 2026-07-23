using Application.ViewModels;
using MediatR;

namespace Application.Features.WarehouseItems.UpdateProductQuantity;

public record AdjustItemQuantityCommand(
    string Id,
    int Quantity,
    string Location)
    : IRequest<WarehouseItemViewModel>;