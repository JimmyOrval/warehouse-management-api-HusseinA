using AutoMapper;
using Domain.Events.Contracts;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Storage;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Suppliers.Commands.UploadSupplierDocument;

public class UploadSupplierDocumentCommandHandler(
    ISupplierRepository supplierRepository,
    IFileStorageService fileStorageService,
    IEventPublisher eventPublisher,
    IOptions<MinIoStorage> minIoOptions,
    IMapper mapper,
    ILogger<UploadSupplierDocumentCommandHandler> logger)
    : IRequestHandler<UploadSupplierDocumentCommand, string>
{
    public async Task<string> Handle(UploadSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
        {
            logger.LogWarning("Document upload failed: supplier {SupplierId} not found", request.SupplierId);
            throw new NotFoundException($"Supplier '{request.SupplierId}' not found");
        }

        var uploaded = await fileStorageService.UploadAsync(
            request.File, request.FileName, request.ContentType, cancellationToken);

        var document = new SupplierDocument
        {
            Id = Guid.NewGuid().ToString(),
            SupplierId = supplier.Id,
            FileName = uploaded.FileName,
            ObjectKey = uploaded.ObjectKey,
            ContentType = uploaded.ContentType,
            Size = uploaded.Size
        };

        supplierRepository.AddDocument(document);
        await supplierRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Document uploaded for supplier {SupplierId}, object key {ObjectKey}",
            supplier.Id, uploaded.ObjectKey);
        
        await eventPublisher.PublishAsync(new WarehouseFileUploaded
        {
            CorrelationId = Guid.NewGuid().ToString(),
            EventType = "FileUploaded",
            RelatedEntityId = supplier.Id,
            RelatedEntityType = "Supplier",
            Severity = "Info",
            FileName = uploaded.FileName,
            FileType = "SupplierDocument",
            BucketName = minIoOptions.Value.BucketName,
        }, "file.uploaded", cancellationToken);

        logger.LogInformation(
            "Published WarehouseFileUploaded for supplier {SupplierId}",
            supplier.Id);

        return document.Id;
    }
}