using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.GetOutOfStockProducts;

public record GetOutOfStockProductsQuery() : IRequest<IEnumerable<ProductViewModel>>;
