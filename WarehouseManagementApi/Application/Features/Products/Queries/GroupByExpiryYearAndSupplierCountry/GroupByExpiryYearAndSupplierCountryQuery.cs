using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;

public record GroupByExpiryYearAndSupplierCountryQuery() : IRequest<IEnumerable<ProductViewModel>>;