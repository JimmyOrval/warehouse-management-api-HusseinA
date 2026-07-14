using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.ArchiveProduct;

public record ArchiveProductCommand(
    [property: GuidString]
    string Id)
    : IRequest<ProductViewModel>;