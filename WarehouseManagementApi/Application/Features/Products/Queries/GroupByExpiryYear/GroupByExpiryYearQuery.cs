using Application.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GroupByExpiryYear;

public record GroupByExpiryYearQuery() : IRequest<IEnumerable<ProductDto>>;