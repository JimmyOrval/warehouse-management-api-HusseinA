using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.UploadProductImage;

public record UploadProductImageCommand(
    [property: GuidString]
    string ProductId,
    Stream Image,
    long ImageLength,
    string FileName)
    : IRequest<ProductImageViewModel>;