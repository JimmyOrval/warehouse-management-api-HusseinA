using MediatR;

namespace Application.Features.Suppliers.Commands.DeleteSupplierDocument;

public record DeleteSupplierDocumentCommand(string DocumentId) : IRequest;