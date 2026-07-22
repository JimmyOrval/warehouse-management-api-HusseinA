using Application.Common;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Suppliers.Queries.DownloadSupplierDocument;

public class DownloadSupplierDocumentQueryHandler(
    ISupplierRepository supplierRepository,
    IFileStorageService fileStorageService)
    : IRequestHandler<DownloadSupplierDocumentQuery, DownloadedFileResult>
{
    public async Task<DownloadedFileResult> Handle(DownloadSupplierDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await supplierRepository.GetDocumentByIdAsync(request.DocumentId, cancellationToken)
                       ?? throw new NotFoundException($"Document {request.DocumentId} not found");

        var (content, contentType) = await fileStorageService.DownloadAsync(document.ObjectKey, cancellationToken);
        return new DownloadedFileResult(content, contentType, document.FileName);
    }
}