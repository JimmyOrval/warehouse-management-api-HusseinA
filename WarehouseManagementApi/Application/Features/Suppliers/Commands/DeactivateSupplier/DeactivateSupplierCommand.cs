using Domain.Models;
using MediatR;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public record DeactivateSupplierCommand(string Id) : IRequest<Supplier>;