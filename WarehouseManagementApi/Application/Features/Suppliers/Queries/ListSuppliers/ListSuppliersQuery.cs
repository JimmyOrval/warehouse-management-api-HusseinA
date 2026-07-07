using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public record ListSuppliersQuery() : IRequest<IEnumerable<Supplier>>;