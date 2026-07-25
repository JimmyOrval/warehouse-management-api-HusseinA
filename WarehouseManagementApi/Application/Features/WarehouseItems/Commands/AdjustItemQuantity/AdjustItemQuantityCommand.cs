using Application.ViewModels;
using MediatR;

namespace Application.Features.WarehouseItems.Commands.AdjustItemQuantity;

public record AdjustItemQuantityCommand(
    string ItemId,
    int Quantity)
    : IRequest<WarehouseItemViewModel>;