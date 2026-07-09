using Application.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;

public record GroupByExpiryYearAndSupplierCountryQuery() : IRequest<IEnumerable<ProductDto>>;