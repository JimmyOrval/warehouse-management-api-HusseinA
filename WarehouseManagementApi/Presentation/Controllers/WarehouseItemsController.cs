using Application.Features.WarehouseItems.Commands.CreateWarehouseItem;
using Application.Features.WarehouseItems.Commands.DeleteWarehouseItem;
using Application.Features.WarehouseItems.Commands.UpdateProductQuantity;
using Application.Features.WarehouseItems.Queries.GetItemStockMovements;
using Application.Features.WarehouseItems.Queries.GetWarehouseItemById;
using Application.Features.WarehouseItems.Queries.ListWarehouseItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Policy = "AuthenticatedUser")]
[ApiController]
[Route("api/warehouse-items")]
public class WarehouseItemsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllItems(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ListWarehouseItemsQuery(), cancellationToken));
    }

    [HttpGet("{itemId}")]
    public async Task<IActionResult> GetItemById(string itemId, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetWarehouseItemByIdQuery(itemId), cancellationToken));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateItem(
        [FromBody] CreateWarehouseItemCommand command,
        CancellationToken cancellationToken)
    {
        var itemId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetItemById), new { itemId }, new { id = itemId });
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{itemId}/quantity")]
    public async Task<IActionResult> AdjustItemQuantity(
        [FromRoute] string itemId,
        [FromQuery] int quantity,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new AdjustItemQuantityCommand(itemId, quantity), cancellationToken));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{itemId}")]
    public async Task<IActionResult> DeleteItem(string itemId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteWarehouseItemCommand(itemId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{itemId}/movements")]
    public async Task<IActionResult> GetItemStockMovements(
        [FromRoute] string itemId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetItemStockMovementsQuery(itemId), cancellationToken));
    }
}