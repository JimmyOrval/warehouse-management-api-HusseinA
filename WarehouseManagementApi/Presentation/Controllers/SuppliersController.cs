using Application.Features.Products.Commands.DeleteProductImage;
using Application.Features.Suppliers.Commands.AssignSupplierToProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Features.Suppliers.Commands.DeactivateSupplier;
using Application.Features.Suppliers.Commands.DeleteSupplierDocument;
using Application.Features.Suppliers.Commands.UploadSupplierDocument;
using Application.Features.Suppliers.Queries.DownloadSupplierDocument;
using Application.Features.Suppliers.Queries.GetSupplierById;
using Application.Features.Suppliers.Queries.ListSuppliers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace Presentation.Controllers;

[Authorize(Policy = "AuthenticatedUser")]
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

    [Authorize(Policy = "AdminOnly")]
    [ServiceFilter(typeof(ModelValidationFilter))]
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierCommand command,
        CancellationToken cancellationToken)
    {
        var supplierId = await mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetSupplier), new { id = supplierId }, null);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(string id,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new DeactivateSupplierCommand(id),
            cancellationToken));
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public async Task<IActionResult> AssignSupplier(
        [FromRoute] string id,
        [FromRoute] string supplierId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new AssignSupplierToProductCommand(id, supplierId),
            cancellationToken));
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpPost("{supplierId}/document")]
    public async Task<IActionResult> UploadDocument([FromRoute] string documentId, IFormFile document,
        CancellationToken cancellationToken)
    {
        await using var stream = document.OpenReadStream();
        return Ok(await mediator.Send(new UploadSupplierDocumentCommand(
            documentId, stream, document.Length, document.FileName, document.ContentType), cancellationToken));
    }

    [HttpGet("document/{documentId}")]
    public async Task<IActionResult> DownloadDocument(
        [FromRoute] string documentId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DownloadSupplierDocumentQuery(documentId), cancellationToken);
        return File(result.Content, result.ContentType, result.FileName);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("document/{documentId}")]
    public async Task<IActionResult> DeleteDocument([FromRoute] string documentId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteSupplierDocumentCommand(documentId), cancellationToken);
        return NoContent();
    }
}