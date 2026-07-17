using Application.ViewModels;
using MediatR;

namespace Application.Features.Suppliers.Commands.AssignSupplierToProduct;

public record AssignSupplierToProductCommand(
    string Id,
    string SupplierId)
    : IRequest<ProductViewModel>;