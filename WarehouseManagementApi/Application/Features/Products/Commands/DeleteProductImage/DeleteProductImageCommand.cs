using MediatR;

namespace Application.Features.Products.Commands.DeleteProductImage;

public record DeleteProductImageCommand(string ImageId) : IRequest;