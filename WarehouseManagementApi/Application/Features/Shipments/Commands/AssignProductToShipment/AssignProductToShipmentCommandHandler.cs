using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Shipments.Commands.AssignProductToShipment;

public class AssignProductToShipmentCommandHandler(
    IShipmentRepository shipmentRepository,
    IProductRepository productRepository,
    IMapper mapper,
    ILogger<AssignProductToShipmentCommandHandler> logger)
    : IRequestHandler<AssignProductToShipmentCommand, ShipmentViewModel>
{
    public async Task<ShipmentViewModel> Handle(AssignProductToShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);
        if (shipment == null)
        {
            logger.LogWarning("Assign product failed: shipment {ShipmentId} not found", request.ShipmentId);
            throw new NotFoundException($"Shipment '{request.ShipmentId}' not found");
        }

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            logger.LogWarning("Assign product failed: product {ProductId} not found", request.ProductId);
            throw new NotFoundException($"Product '{request.ProductId}' not found");
        }

        shipment.AssignProduct(request.ProductId, request.Quantity);
        await shipmentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} assigned to shipment {ShipmentId}",
            request.ProductId, request.ShipmentId);

        return mapper.Map<ShipmentViewModel>(shipment);
    }
}
