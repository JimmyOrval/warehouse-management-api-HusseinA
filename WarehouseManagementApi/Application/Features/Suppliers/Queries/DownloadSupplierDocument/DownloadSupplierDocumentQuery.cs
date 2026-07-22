using Application.Common;
using MediatR;

namespace Application.Features.Suppliers.Queries.DownloadSupplierDocument;

public record DownloadSupplierDocumentQuery(string DocumentId) : IRequest<DownloadedFileResult>;