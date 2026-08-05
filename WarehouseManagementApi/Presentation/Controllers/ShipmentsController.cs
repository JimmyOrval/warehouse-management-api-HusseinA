using Application.Features.Shipments.Commands.AssignProductToShipment;
using Application.Features.Shipments.Commands.CreateShipment;
using Application.Features.Shipments.Commands.UpdateShipmentStatus;
using Application.Features.Shipments.Queries.GetShipmentById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Policy = "AuthenticatedUser")]
[ApiController]
[Route("api/[controller]")]
public class ShipmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipmentById([FromRoute] string id,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetShipmentByIdQuery(id), cancellationToken));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentCommand command,
        CancellationToken cancellationToken)
    {
        var shipmentId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetShipmentById), new { id = shipmentId }, null);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("{id}/products")]
    public async Task<IActionResult> AssignProduct([FromRoute] string id,
        [FromBody] AssignProductRequest request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new AssignProductToShipmentCommand(id, request.ProductId, request.Quantity),
            cancellationToken));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] string id,
        [FromBody] UpdateShipmentStatusRequest request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new UpdateShipmentStatusCommand(id, request.NewStatus),
            cancellationToken));
    }
}

// small route-level request shapes so the command records don't need the route id
// duplicated in the body - matches how AssignSupplierToProduct takes its ids from the
// route rather than the body
public record AssignProductRequest(string ProductId, int Quantity);
public record UpdateShipmentStatusRequest(Domain.Enums.ShipmentStatus NewStatus);
