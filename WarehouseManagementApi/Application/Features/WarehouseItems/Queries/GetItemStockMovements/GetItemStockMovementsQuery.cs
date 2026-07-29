using Application.ViewModels;
using MediatR;

namespace Application.Features.WarehouseItems.Queries.GetItemStockMovements;

public record GetItemStockMovementsQuery(string ItemId) : IRequest<List<StockMovementViewModel>>;