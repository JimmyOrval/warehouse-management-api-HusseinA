using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYear;

public record GroupByExpiryYearQuery() : IRequest<IEnumerable<ProductViewModel>>;