using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Shipments.Queries.GetShipmentById;

public class GetShipmentByIdQueryHandler(
    IShipmentRepository shipmentRepository,
    IMapper mapper)
    : IRequestHandler<GetShipmentByIdQuery, ShipmentViewModel>
{
    public async Task<ShipmentViewModel> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (shipment == null)
            throw new NotFoundException($"Shipment '{request.Id}' not found");

        return mapper.Map<ShipmentViewModel>(shipment);
    }
}
