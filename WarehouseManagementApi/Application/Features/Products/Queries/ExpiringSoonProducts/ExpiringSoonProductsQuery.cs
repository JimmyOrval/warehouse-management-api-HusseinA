using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.ExpiringSoonProducts;

public record ExpiringSoonProductsQuery() : IRequest<IEnumerable<ProductViewModel>>;