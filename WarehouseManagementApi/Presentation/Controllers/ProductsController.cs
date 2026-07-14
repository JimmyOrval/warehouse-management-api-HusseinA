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
    public async Task<IActionResult> GetProducts([FromQuery] bool? onlyAvailable = true)
    {
        return Ok(await mediator.Send(new ListProductsQuery(onlyAvailable)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] string id)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id));
        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? supplier)
    {
        return Ok(await mediator.Send(new SearchProductsQuery(name, supplier)));
    }

    [ServiceFilter(typeof(ModelValidationFilter))]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await mediator.Send(command);
        
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, null);
    }

    [HttpPut("{id}/quantity/{location}")]
    public async Task<IActionResult> UpdateQuantity([FromRoute] string id,
        [FromBody] int quantity, [FromRoute] string location)
    {
        return Ok(await mediator.Send(new
            UpdateProductQuantityCommand(
                id, quantity, location)));
    }

    [HttpPut("{id}/price")]
    public async Task<IActionResult> UpdatePrice([FromRoute] string id, [FromBody] decimal newPrice)
    {
        return Ok(await mediator.Send(new UpdateProductPriceCommand(id, newPrice)));
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(string id, IFormFile image)
    {
        await using var stream = image.OpenReadStream();
        return Ok(await mediator.Send(new UploadProductImageCommand(
            id, stream, image.Length, image.FileName)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id)
    {
        await mediator.Send(new ArchiveProductCommand(id));
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
        [FromQuery] bool isAscending)
    {
        return Ok(await mediator.Send(new GetProductsBySupplierQuery
            (supplierName, isAscending)));
    }

    [HttpGet("year")]
    public async Task<IActionResult> GroupByExpiryYear()
    {
        return Ok(await mediator.Send(new GroupByExpiryYearQuery()));
    }

    [HttpGet("year/country")]
    public async Task<IActionResult> GroupByExpiryYearAndSupplierCountry()
    {
        return Ok(await mediator.Send(new GroupByExpiryYearAndSupplierCountryQuery()));
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        return Ok(await mediator.Send(new GetProductCountQuery()));
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetProductsByPage([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        return Ok(await mediator.Send(new GetPagedProductsQuery(pageNumber, pageSize)));
    }
}