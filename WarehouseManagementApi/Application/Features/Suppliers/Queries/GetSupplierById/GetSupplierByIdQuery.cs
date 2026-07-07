using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(string Id) : IRequest<Supplier>;