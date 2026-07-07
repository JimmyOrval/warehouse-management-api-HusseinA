using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.ListProducts;

public record ListProductsQuery(bool? OnlyAvailable) : IRequest<IEnumerable<Product>>;