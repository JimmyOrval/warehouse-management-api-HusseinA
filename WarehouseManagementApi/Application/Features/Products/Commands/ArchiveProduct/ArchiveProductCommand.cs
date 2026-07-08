using Application.DTOs;
using MediatR;

namespace Application.Features.Products.Commands.ArchiveProduct;

public record ArchiveProductCommand(string Id) : IRequest<ProductDto>;