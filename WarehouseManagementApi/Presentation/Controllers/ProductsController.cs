using Application.Features.Products.Commands.ArchiveProduct;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.DeleteProductImage;
using Application.Features.Products.Commands.UpdateProductPrice;
using Application.Features.Products.Commands.UploadProductImage;
using Application.Features.Products.Queries.DownloadProductImage;
using Application.Features.Products.Queries.GetPagedProducts;
using Application.Features.Products.Queries.GetProductById;
using Application.Features.Products.Queries.GetProductCount;
using Application.Features.Products.Queries.GetProductsBySupplier;
using Application.Features.Products.Queries.GetProductWarehouseItems;
using Application.Features.Products.Queries.GetTotalProductQuantity;
using Application.Features.Products.Queries.GroupByExpiryYear;
using Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;
using Application.Features.Products.Queries.ListProducts;
using Application.Features.Products.Queries.SearchProducts;
using Application.Features.WarehouseItems.Queries.GetWarehouseItemById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace Presentation.Controllers;

[Authorize(Policy = "AuthenticatedUser")]
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken,
        [FromQuery] bool? onlyAvailable = true)
    {
        return Ok(await mediator.Send(new ListProductsQuery(onlyAvailable),
            cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id),
            cancellationToken);
        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name,
        [FromQuery] string? supplier,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new SearchProductsQuery(name, supplier),
            cancellationToken));
    }

    [Authorize(Policy = "AdminOnly")]
    [ServiceFilter(typeof(ModelValidationFilter))]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var productId = await mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, null);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}/price")]
    public async Task<IActionResult> UpdatePrice([FromRoute] string id,
        [FromBody] decimal newPrice, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new UpdateProductPriceCommand(id, newPrice),
            cancellationToken));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("{productId}/image")]
    public async Task<IActionResult> UploadImage([FromRoute] string productId, IFormFile image,
        CancellationToken cancellationToken)
    {
        await using var stream = image.OpenReadStream();
        return Ok(await mediator.Send(new UploadProductImageCommand(
            productId, stream, image.Length, image.FileName, image.ContentType), cancellationToken));
    }

    [HttpGet("image/{imageId}")]
    public async Task<IActionResult> DownloadImage(
        [FromRoute] string imageId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DownloadProductImageQuery(imageId), cancellationToken);
        return File(result.Content, result.ContentType, result.FileName);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("image/{imageId}")]
    public async Task<IActionResult> DeleteImage([FromRoute] string imageId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductImageCommand(imageId), cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ArchiveProductCommand(id), cancellationToken);
        return NoContent();
    }
    
    // made it so anyone can use this
    [AllowAnonymous]
    [HttpGet("server-time")]
    public IActionResult GetServerTime([FromHeader(Name = "Accept-Language")] string language)
    {
        var lang = string.IsNullOrEmpty(language) ? "en-US" : language.Split(',')[0].Trim();

        var now = DateTime.Now;

        var result = lang switch
        {
            "en-US" =>
                // month/day/year
                now.Month + "/" + now.Day + "/" + now.Year,
            "fr-FR" or "ar-LB" =>
                // day/month/year
                now.Day + "/" + now.Month + "/" + now.Year,
            _ => now.Year + "-" + now.Month + "-" + now.Day
        };

        return Ok(result);
    }
    
    [HttpGet("supplier")]
    public async Task<IActionResult> GetProductsBySupplier(
        [FromQuery] string supplierName,
        [FromQuery] bool isAscending,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetProductsBySupplierQuery
            (supplierName, isAscending), cancellationToken));
    }

    [HttpGet("year")]
    public async Task<IActionResult> GroupByExpiryYear(
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GroupByExpiryYearQuery(),
            cancellationToken));
    }

    [HttpGet("year/country")]
    public async Task<IActionResult> GroupByExpiryYearAndSupplierCountry(
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GroupByExpiryYearAndSupplierCountryQuery(),
            cancellationToken));
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetProductCountQuery(), cancellationToken));
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetProductsByPage(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetPagedProductsQuery(pageNumber, pageSize),
            cancellationToken));
    }

    [HttpGet("{productId}/items")]
    public async Task<IActionResult> GetProductWarehouseItems(
        [FromRoute] string productId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetProductWarehouseItemsQuery(productId), cancellationToken));
    }

    [HttpGet("{productId}/quantity")]
    public async Task<IActionResult> GetTotalStockQuantity([FromRoute] string productId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetTotalProductQuantityQuery(productId), cancellationToken));
    }
}