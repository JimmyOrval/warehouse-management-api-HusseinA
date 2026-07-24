using Application.Features.Products.Commands.DeleteProductImage;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Commands.DeleteSupplierDocument;

public class DeleteSupplierDocumentCommandHandler(
    ISupplierRepository supplierRepository,
    IFileStorageService fileStorageService,
    ILogger<DeleteProductImageCommandHandler> logger)
    : IRequestHandler<DeleteSupplierDocumentCommand>
{
    public async Task Handle(DeleteSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await supplierRepository.GetDocumentByIdAsync(
                        request.DocumentId, cancellationToken)
                    ?? throw new NotFoundException(
                        $"Document {request.DocumentId} not found");
        
        await fileStorageService.DeleteAsync(document.ObjectKey, cancellationToken);
        supplierRepository.DeleteDocument(document);
        await supplierRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Document {DocumentId} deleted for supplier {DocumentSupplierId}",
            request.DocumentId, document.SupplierId);
    }
}