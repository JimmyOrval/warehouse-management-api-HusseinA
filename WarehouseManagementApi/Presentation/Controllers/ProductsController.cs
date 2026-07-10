using Application.Contracts;
using Application.Features.Products.Commands.ArchiveProduct;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.UpdateProductPrice;
using Application.Features.Products.Commands.UpdateProductQuantity;
using Application.Features.Products.Commands.UploadProductImage;
using Application.Features.Products.Queries.GetProductById;
using Application.Features.Products.Queries.ListProducts;
using Application.Features.Products.Queries.SearchProducts;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts([FromQuery] bool? onlyAvailable = true)
    {
        return Ok(mediator.Send(new ListProductsQuery(onlyAvailable)));
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById([FromRoute] string id)
    {
        var product = mediator.Send(new GetProductByIdQuery(id));
        return Ok(product);
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? name, [FromQuery] string? supplier)
    {
        return Ok(mediator.Send(new SearchProductsQuery(name, supplier)));
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = mediator.Send(command);
        
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, null);
    }

    [HttpPut("{id}/quantity")]
    public IActionResult UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request)
    {
        return Ok(mediator.Send(new UpdateProductQuantityCommand(id, request.Quantity, request.Location)));
    }

    [HttpPut("{id}/price")]
    public IActionResult UpdatePrice([FromRoute] string id, [FromBody] decimal newPrice)
    {
        return Ok(mediator.Send(new UpdateProductPriceCommand(id, newPrice)));
    }

    [HttpPost("{id}/image")]
    public IActionResult UploadImage(string id, IFormFile image)
    {
        using var stream = image.OpenReadStream();
        return Ok(mediator.Send(new UploadProductImageCommand(
            id, stream, image.Length, image.FileName)));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct([FromRoute] string id)
    {
        mediator.Send(new ArchiveProductCommand(id));
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
}