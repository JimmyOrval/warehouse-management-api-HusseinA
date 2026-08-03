using Application.ViewModels;
using Domain.Enums;
using MediatR;

namespace Application.Features.Shipments.Commands.UpdateShipmentStatus;

public record UpdateShipmentStatusCommand(
    string ShipmentId,
    ShipmentStatus NewStatus) : IRequest<ShipmentViewModel>;
