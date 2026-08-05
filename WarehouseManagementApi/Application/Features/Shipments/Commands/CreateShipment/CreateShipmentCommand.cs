using MediatR;

namespace Application.Features.Shipments.Commands.CreateShipment;

public record CreateShipmentCommand(string SupplierId) : IRequest<string>;
