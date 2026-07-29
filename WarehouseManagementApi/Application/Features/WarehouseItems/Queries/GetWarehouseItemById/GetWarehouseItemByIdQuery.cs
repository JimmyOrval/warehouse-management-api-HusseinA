using Application.ViewModels;
using MediatR;

namespace Application.Features.WarehouseItems.Queries.GetWarehouseItemById;

public record GetWarehouseItemByIdQuery(string ItemId) : IRequest<WarehouseItemViewModel>;