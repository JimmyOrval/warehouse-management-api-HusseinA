using Application.Common;
using MediatR;

namespace Application.Features.Suppliers.Commands.UploadSupplierDocument;

public record UploadSupplierDocumentCommand(
    string SupplierId,
    Stream File,
    long FileLength,
    string FileName,
    string ContentType)
    : IRequest<string>;