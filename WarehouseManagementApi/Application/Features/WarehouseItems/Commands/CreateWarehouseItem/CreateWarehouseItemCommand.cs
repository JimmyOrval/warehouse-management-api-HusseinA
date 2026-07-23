using MediatR;

namespace Application.Features.WarehouseItems.Commands.CreateWarehouseItem;

public record CreateWarehouseItemCommand(
    string ProductId,
    string Location) 
    : IRequest<string>;