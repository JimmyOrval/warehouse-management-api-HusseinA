using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand
(
    string Name,
    string Sku,
    string Description,
    decimal Price,
    string SupplierId,
    DateTime ExpiryDate
) : IRequest<string>;