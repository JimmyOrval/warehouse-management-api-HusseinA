using Application.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetProductsBySupplier;

public record GetProductsBySupplierQuery(string SupplierName, bool IsAscending)
    : IRequest<IEnumerable<ProductDto>>;