using Application.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.ListProducts;

public record ListProductsQuery(bool? OnlyAvailable) : IRequest<IEnumerable<ProductDto>>;