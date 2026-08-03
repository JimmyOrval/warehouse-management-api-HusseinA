using Application.ViewModels;
using MediatR;

namespace Application.Features.Shipments.Commands.AssignProductToShipment;

public record AssignProductToShipmentCommand(
    string ShipmentId,
    string ProductId,
    int Quantity) : IRequest<ShipmentViewModel>;
