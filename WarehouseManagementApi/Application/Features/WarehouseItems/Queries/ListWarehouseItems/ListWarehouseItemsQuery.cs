using Application.ViewModels;
using MediatR;

namespace Application.Features.WarehouseItems.Queries.ListWarehouseItems;

public record ListWarehouseItemsQuery() : IRequest<List<WarehouseItemViewModel>>;