using MediatR;

namespace Application.Features.Products.Commands.UploadProductImage;

public record UploadProductImageCommand(
    string ProductId,
    Stream Image,
    long ImageLength,
    string FileName,
    string ContentType)
    : IRequest<string>;