using Application.Features.Products.Commands.ArchiveProduct;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.UpdateProductPrice;
using Application.Features.Products.Commands.UpdateProductQuantity;
using Application.Features.Products.Commands.UploadProductImage;
using Application.Features.Products.Queries.GetPagedProducts;
using Application.Features.Products.Queries.GetProductById;
using Application.Features.Products.Queries.GetProductCount;
using Application.Features.Products.Queries.GetProductsBySupplier;
using Application.Features.Products.Queries.GroupByExpiryYear;
using Application.Features.Products.Queries.GroupByExpiryYearAndSupplierCountry;
using Application.Features.Products.Queries.ListProducts;
using Application.Features.Products.Queries.SearchProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] bool? onlyAvailable = true,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new ListProductsQuery(onlyAvailable),
            cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] string id,
        CancellationToken cancellationToken = default)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id),
            cancellationToken);
        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name,
        [FromQuery] string? supplier,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new SearchProductsQuery(name, supplier),
            cancellationToken));
    }

    [ServiceFilter(typeof(ModelValidationFilter))]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var productId = await mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, null);
    }

    [HttpPut("{id}/quantity/{location}")]
    public async Task<IActionResult> UpdateQuantity([FromRoute] string id,
        [FromBody] int quantity, [FromRoute] string location,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new
            UpdateProductQuantityCommand(
                id, quantity, location),
            cancellationToken));
    }

    [HttpPut("{id}/price")]
    public async Task<IActionResult> UpdatePrice([FromRoute] string id,
        [FromBody] decimal newPrice, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new UpdateProductPriceCommand(id, newPrice),
            cancellationToken));
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(string id, IFormFile image,
        CancellationToken cancellationToken = default)
    {
        await using var stream = image.OpenReadStream();
        return Ok(await mediator.Send(new UploadProductImageCommand(
            id, stream, image.Length, image.FileName), cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new ArchiveProductCommand(id), cancellationToken);
        return NoContent();
    }
    
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
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetProductsBySupplierQuery
            (supplierName, isAscending), cancellationToken));
    }

    [HttpGet("year")]
    public async Task<IActionResult> GroupByExpiryYear(
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GroupByExpiryYearQuery(),
            cancellationToken));
    }

    [HttpGet("year/country")]
    public async Task<IActionResult> GroupByExpiryYearAndSupplierCountry(
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GroupByExpiryYearAndSupplierCountryQuery(),
            cancellationToken));
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount(CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetProductCountQuery(), cancellationToken));
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetProductsByPage(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetPagedProductsQuery(pageNumber, pageSize),
            cancellationToken));
    }
}