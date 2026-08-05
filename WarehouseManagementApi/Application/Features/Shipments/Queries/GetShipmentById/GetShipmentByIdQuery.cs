using Application.ViewModels;
using MediatR;

namespace Application.Features.Shipments.Queries.GetShipmentById;

public record GetShipmentByIdQuery(string Id) : IRequest<ShipmentViewModel>;
