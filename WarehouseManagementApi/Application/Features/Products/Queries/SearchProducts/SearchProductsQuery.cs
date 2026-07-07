using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(string? Name, string? Supplier) : IRequest<List<Product>>;