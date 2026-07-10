using Application.ViewModels;
using MediatR;

namespace Application.Features.Suppliers.Queries.ListSuppliers;

public record ListSuppliersQuery() : IRequest<IEnumerable<SupplierViewModel>>;