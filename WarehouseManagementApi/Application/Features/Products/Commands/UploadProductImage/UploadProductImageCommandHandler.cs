using AutoMapper;
using Domain.Events.Contracts;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Storage;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Products.Commands.UploadProductImage;

public class UploadProductImageCommandHandler(
    IProductRepository productRepository,
    IFileStorageService fileStorageService,
    IEventPublisher eventPublisher,
    IOptions<MinIoStorage> minIoOptions,
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
        
        await eventPublisher.PublishAsync(new WarehouseFileUploaded
        {
            CorrelationId = Guid.NewGuid().ToString(),
            EventType = "FileUploaded",
            RelatedEntityId = product.Id,
            RelatedEntityType = "Product",
            Severity = "Info",
            FileName = uploaded.FileName,
            FileType = "ProductImage",
            BucketName = minIoOptions.Value.BucketName,
        }, "file.uploaded", cancellationToken);

        logger.LogInformation(
            "Published WarehouseFileUploaded for product {ProductId}",
            product.Id);
        
        return productImage.Id;
    }
}