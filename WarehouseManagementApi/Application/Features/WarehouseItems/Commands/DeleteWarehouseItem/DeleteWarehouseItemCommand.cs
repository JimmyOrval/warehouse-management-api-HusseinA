using MediatR;

namespace Application.Features.WarehouseItems.Commands.DeleteWarehouseItem;

public record DeleteWarehouseItemCommand(string Id) : IRequest;