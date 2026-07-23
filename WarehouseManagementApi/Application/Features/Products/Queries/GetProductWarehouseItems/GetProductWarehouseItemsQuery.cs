using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.GetProductWarehouseItems;

public record GetProductWarehouseItemsQuery(string ProductId) : IRequest<List<WarehouseItemViewModel>>;