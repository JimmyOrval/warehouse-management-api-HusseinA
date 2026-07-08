using MediatR;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(
    string Name,
    string Country,
    string ContactEmail,
    string Phone) : IRequest<string>;