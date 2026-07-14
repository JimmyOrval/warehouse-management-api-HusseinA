using Application.Validation;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(
    [property: GuidString]
    string Id) : IRequest<SupplierViewModel>;