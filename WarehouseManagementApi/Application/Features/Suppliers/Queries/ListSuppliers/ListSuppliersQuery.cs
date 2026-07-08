using Application.DTOs;
using MediatR;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public record ListSuppliersQuery() : IRequest<IEnumerable<SupplierDto>>;