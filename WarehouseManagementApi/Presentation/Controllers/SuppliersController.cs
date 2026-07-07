using Application.Contracts;
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
    public IActionResult CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var supplierId = mediator.Send(new CreateSupplierCommand(request));
        return CreatedAtAction(nameof(GetSupplier), new { id = supplierId }, null);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSupplier(string id)
    {
        return Ok(mediator.Send(new DeactivateSupplierCommand(id)));
    }
}