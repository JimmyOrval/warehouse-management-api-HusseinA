using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Commands.UploadSupplierDocument;

public class UploadSupplierDocumentCommandHandler(
    ISupplierRepository supplierRepository,
    IFileStorageService fileStorageService,
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

        return document.Id;
    }
}