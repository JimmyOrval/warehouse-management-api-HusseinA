using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.ArchiveProduct;

public record ArchiveProductCommand(string Id) : IRequest<ProductViewModel>;