using Application.DTOs;
using MediatR;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public record DeactivateSupplierCommand(string Id) : IRequest<SupplierDto>;