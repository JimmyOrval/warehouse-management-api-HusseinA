using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.ArchiveProduct;

public record ArchiveProductCommand(string Id) : IRequest<Product>;