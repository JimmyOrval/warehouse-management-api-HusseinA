using Application.Features.Suppliers.Commands.AssignSupplierToProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Features.Suppliers.Commands.DeactivateSupplier;
using Application.Features.Suppliers.Queries.GetSupplierById;
using Application.Features.Suppliers.Queries.ListSuppliers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSuppliers(
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ListSuppliersQuery(),
            cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplier(string id,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetSupplierByIdQuery(id),
            cancellationToken));
    }

    [ServiceFilter(typeof(ModelValidationFilter))]
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierCommand command,
        CancellationToken cancellationToken)
    {
        var supplierId = await mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetSupplier), new { id = supplierId }, null);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(string id,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new DeactivateSupplierCommand(id),
            cancellationToken));
    }
    
    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public async Task<IActionResult> AssignSupplier(
        [FromRoute] string id,
        [FromRoute] string supplierId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new AssignSupplierToProductCommand(id, supplierId),
            cancellationToken));
    }
}