using Application.Common;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Queries.DownloadProductImage;

public class DownloadProductImageQueryHandler(
    IProductRepository productRepository,
    IFileStorageService fileStorageService)
    : IRequestHandler<DownloadProductImageQuery, DownloadedFileResult>
{
    public async Task<DownloadedFileResult> Handle(DownloadProductImageQuery request, CancellationToken cancellationToken)
    {
        var image = await productRepository.GetImageByIdAsync(
                        request.ImageId, cancellationToken)
                    ?? throw new NotFoundException($"Image {request.ImageId} not found");

        var (content, contentType) = await
            fileStorageService.DownloadAsync(
                image.ObjectKey, cancellationToken);
        return new DownloadedFileResult(content, contentType, image.FileName);
    }
}