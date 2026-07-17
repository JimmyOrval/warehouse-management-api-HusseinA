using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public record DeactivateSupplierCommand(
    [property: GuidString]
    string Id)
    : IRequest<SupplierViewModel>;