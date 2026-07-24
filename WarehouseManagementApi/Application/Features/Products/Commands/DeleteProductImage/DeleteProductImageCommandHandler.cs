using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandler(
    IProductRepository productRepository,
    IFileStorageService fileStorageService,
    ILogger<DeleteProductImageCommandHandler> logger)
    : IRequestHandler<DeleteProductImageCommand>
{
    public async Task Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var image = await productRepository.GetImageByIdAsync(
                        request.ImageId, cancellationToken)
                    ?? throw new NotFoundException(
                        $"Image {request.ImageId} not found");
        
        await fileStorageService.DeleteAsync(image.ObjectKey, cancellationToken);
        productRepository.DeleteImage(image);
        await productRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Image {ImageId} deleted for product {ImageProductId}",
            request.ImageId, image.ProductId);
    }
}