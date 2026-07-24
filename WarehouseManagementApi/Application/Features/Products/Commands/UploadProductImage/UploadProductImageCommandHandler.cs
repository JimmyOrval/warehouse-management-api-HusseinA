using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.UploadProductImage;

public class UploadProductImageCommandHandler(
    IProductRepository productRepository,
    IFileStorageService fileStorageService,
    IMapper mapper,
    ILogger<UploadProductImageCommandHandler> logger)
    : IRequestHandler<UploadProductImageCommand, string>
{
    public async Task<string> Handle(UploadProductImageCommand request,
        CancellationToken cancellationToken)
    {
        // check if product exists first
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            logger.LogWarning("Image upload failed: product {ProductId} not found", request.ProductId);
            throw new NotFoundException($"Product '{request.ProductId}' not found");
        }

        var uploaded = await fileStorageService.UploadAsync(
            request.Image,
            request.FileName,
            request.ContentType,
            cancellationToken);
        
        var productImage = new ProductImage
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = product.Id,
            FileName = uploaded.FileName,
            ObjectKey = uploaded.ObjectKey,
            ContentType = uploaded.ContentType,
            Size = uploaded.Size
        };
        
        productRepository.AddImage(productImage);
        await productRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Image uploaded for product {ProductId}, object key {ObjectKey}",
            product.Id, uploaded.ObjectKey);
        
        return productImage.Id;
    }
}