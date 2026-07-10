using Application.Features.Products.Commands.AssignSupplierToProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Features.Suppliers.Commands.DeactivateSupplier;
using Application.Features.Suppliers.Queries.GetSupplierById;
using Application.Features.Suppliers.Queries.ListSuppliers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public IActionResult GetSuppliers()
    {
        return Ok(mediator.Send(new ListSuppliersQuery()));
    }

    [HttpGet("{id}")]
    public IActionResult GetSupplier(string id)
    {
        return Ok(mediator.Send(new GetSupplierByIdQuery(id)));
    }

    [HttpPost]
    public IActionResult CreateSupplier([FromBody] CreateSupplierCommand command)
    {
        var supplierId = mediator.Send(command);
        
        return CreatedAtAction(nameof(GetSupplier), new { id = supplierId }, null);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSupplier(string id)
    {
        return Ok(mediator.Send(new DeactivateSupplierCommand(id)));
    }
    
    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public IActionResult AssignSupplier([FromRoute] string id, [FromRoute] string supplierId)
    {
        return Ok(mediator.Send(new AssignSupplierToProductCommand(id, supplierId)));
    }
}