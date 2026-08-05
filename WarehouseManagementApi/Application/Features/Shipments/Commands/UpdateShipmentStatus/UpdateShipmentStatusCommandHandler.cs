using Application.ViewModels;
using AutoMapper;
using Domain.Events.Contracts;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Shipments.Commands.UpdateShipmentStatus;

public class UpdateShipmentStatusCommandHandler(
    IShipmentRepository shipmentRepository,
    IMapper mapper,
    ILogger<UpdateShipmentStatusCommandHandler> logger)
    : IRequestHandler<UpdateShipmentStatusCommand, ShipmentViewModel>
{
    public async Task<ShipmentViewModel> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);
        if (shipment == null)
        {
            logger.LogWarning("Status update failed: shipment {ShipmentId} not found", request.ShipmentId);
            throw new NotFoundException($"Shipment '{request.ShipmentId}' not found");
        }

        var oldStatus = shipment.Status;
        shipment.UpdateStatus(request.NewStatus);
        await shipmentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Shipment {ShipmentId} status changed from {OldStatus} to {NewStatus}",
            shipment.Id, oldStatus, shipment.Status);

        return mapper.Map<ShipmentViewModel>(shipment);
    }
}
