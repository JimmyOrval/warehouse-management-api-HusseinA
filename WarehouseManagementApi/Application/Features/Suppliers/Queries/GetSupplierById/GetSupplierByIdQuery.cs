using Application.DTOs;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(string Id) : IRequest<SupplierDto>;