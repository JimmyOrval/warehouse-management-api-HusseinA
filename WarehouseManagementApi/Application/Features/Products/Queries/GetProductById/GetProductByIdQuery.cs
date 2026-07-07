using Domain.Models;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(string Id) : IRequest<Product?>;