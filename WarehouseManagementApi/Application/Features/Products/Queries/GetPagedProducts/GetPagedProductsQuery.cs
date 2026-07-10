using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.GetPagedProducts;

public record GetPagedProductsQuery(int PageNumber, int PageSize) : IRequest<IEnumerable<ProductViewModel>>;