using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Shipments.Commands.CreateShipment;

public class CreateShipmentCommandHandler(
    IShipmentRepository shipmentRepository,
    ILogger<CreateShipmentCommandHandler> logger)
    : IRequestHandler<CreateShipmentCommand, string>
{
    public async Task<string> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = new Shipment
        {
            Id = Guid.NewGuid().ToString(),
            SupplierId = request.SupplierId
        };

        shipmentRepository.Add(shipment);
        await shipmentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Shipment {ShipmentId} created for supplier {SupplierId}",
            shipment.Id, shipment.SupplierId);

        return shipment.Id;
    }
}
