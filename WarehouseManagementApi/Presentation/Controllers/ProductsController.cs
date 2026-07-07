using Application.Contracts;
using Application.Features.Products.Commands.ArchiveProduct;
using Application.Features.Products.Commands.AssignSupplierToProduct;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.UpdateProductPrice;
using Application.Features.Products.Commands.UpdateProductQuantity;
using Application.Features.Products.Queries.GetProductById;
using Application.Features.Products.Queries.ListProducts;
using Application.Features.Products.Queries.SearchProducts;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementApi;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
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
    public IActionResult CreateProduct([FromBody] CreateProductRequest request)
    {
        var productId = mediator.Send(new CreateProductCommand(
            request.Name, request.Sku, request.Description,
            request.Price, request.SupplierId, request.ExpiryDate));
        
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
    // removed [FromQuery] and [FromForm] since they caused runtime errors
    public IActionResult UploadImage(string id, IFormFile image)
    {
        // check if product exists first
        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id.Equals(id));
        if (product == null)
            return NotFound("Product not found");
        
        // if file is invalid
        if(image.Length == 0)
            return BadRequest("No image was provided");
        
        // set image size limit
        const long maxFileSize = 2 * 1024 * 1024;
        
        // check if image size exceeds the limit
        if (image.Length > maxFileSize)
            return BadRequest("Image size cannot exceed 2MB");
        
        // get the image's extension
        var extension = Path.GetExtension(image.FileName).ToLower();
        
        // check if extension is valid
        if (!extension.Contains("png") && !extension.Contains("jpg"))
        {
            return BadRequest("Image extension invalid. Use only .jpg or .png");
        }

        // set upload directory
        var uploadFolderPath = Path.GetFullPath("wwwroot/uploads");

        // if directory doesn't exist, create it
        if (!Directory.Exists(uploadFolderPath))
            Directory.CreateDirectory(uploadFolderPath);

        // create a unique file name
        var fileName = $"{Guid.NewGuid()}{extension}";
        // combine full path with new file name
        var filePath = Path.Combine(uploadFolderPath, fileName);

        var productImage = new ProductImage
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = product.Id,
            FileName = fileName,
            FilePath = filePath
        };

        // open a file stream in create mode using our new file path
        using var fileStream = new FileStream(filePath, FileMode.Create);
        // copy the image to the uploads using the file stream
        image.CopyTo(fileStream);
        
        return Ok(productImage);
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

    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public IActionResult AssignSupplier([FromRoute] string id, [FromRoute] string supplierId)
    {
        return Ok(mediator.Send(new AssignSupplierToProductCommand(id, supplierId)));
    }
}