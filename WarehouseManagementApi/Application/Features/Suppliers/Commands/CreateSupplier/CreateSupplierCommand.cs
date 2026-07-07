using Application.Contracts;
using MediatR;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(CreateSupplierRequest Request) : IRequest<string>;