using Application.DTOs;
using MediatR;

namespace Application.Features.Products.Commands.AssignSupplierToProduct;

public record AssignSupplierToProductCommand(string Id, string SupplierId) : IRequest<ProductDto>;