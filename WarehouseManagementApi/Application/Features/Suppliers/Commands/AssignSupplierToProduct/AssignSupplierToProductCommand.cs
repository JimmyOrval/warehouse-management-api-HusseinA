using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.AssignSupplierToProduct;

public record AssignSupplierToProductCommand(
    [property: GuidString]
    string Id,
    [property: GuidString]
    string SupplierId)
    : IRequest<ProductViewModel>;