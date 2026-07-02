using Microsoft.AspNetCore.Mvc;
using WarehouseManagementApi.Contracts;
using WarehouseManagementApi.Models;

namespace WarehouseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public List<Product> GetProducts([FromQuery] bool? onlyAvailable = true)
    {
        var products = FakeWarehouseStore.Products.AsQueryable();
        
        // if query variable is true, filter according to availability
        if (onlyAvailable == true)
        {
            products = products.Where(p => !p.IsArchived && p.QuantityInStock > 0);
        }
        
        // return the list sorted by decreasing creation date
        return products.OrderByDescending(p => p.CreatedAt).ToList();
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById([FromRoute] string id)
    {
        if (id?.Length != 36)
            return BadRequest("Invalid ID format");
        
        var product = FakeWarehouseStore.Products.FirstOrDefault(x => x.Id.Equals(id));

        if (product == null)
            return NotFound();
        
        return Ok(product);
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? name, [FromQuery] string? supplier)
    {
        // BadRequest if both filters are empty
        if(string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
            return BadRequest("Both filters empty. Please enter at least one.");

        var filteredProducts = FakeWarehouseStore.Products.AsEnumerable();

        // if name filter available, filter according to name
        if(!string.IsNullOrWhiteSpace(name))
        {
            filteredProducts = filteredProducts.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        // if name filter available, filter according to supplier
        if(!string.IsNullOrWhiteSpace(supplier))
        {
            filteredProducts = filteredProducts.Where(p => p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase));
        }
        
        return Ok(filteredProducts.ToList());
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductRequest request)
    {
        // check if duplicate SKU already exists
        var skuExists = FakeWarehouseStore.Products
            .Any(p => p.Sku.Equals(request.Sku, StringComparison.OrdinalIgnoreCase));

        if (skuExists)
        {
            // returns code 409
            return Conflict("SKU already exists");
        }
        
        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Sku = request.Sku,
            Description = request.Description,
            Price = request.Price,
            QuantityInStock = request.QuantityInStock,
            SupplierName = request.SupplierName,
            ExpiryDate = request.ExpiryDate,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };
        
        FakeWarehouseStore.Products.Add(product);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}/quantity")]
    public IActionResult UpdateQuantity([FromRoute] string id, [FromBody] int newQuantity)
    {
        // ID should match GUID format
        if (id?.Length != 36)
            return BadRequest("Invalid ID format");

        // quantity cannot be negative
        if (newQuantity < 0)
            return BadRequest("Quantity cannot be negative");

        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id.Equals(id));

        if (product == null)
        {
            return NotFound();
        }
        
        // update both the quantity and updated date
        product.QuantityInStock = newQuantity;
        product.LastUpdatedAt = DateTime.Now;
        return Ok(product);
    }

    [HttpPut("{id}/price")]
    public IActionResult UpdatePrice([FromRoute] string id, [FromBody] decimal newPrice)
    {
        // ID should match GUID format
        if (id?.Length != 36)
            return BadRequest("Invalid ID format");

        // price cannot be negative
        if (newPrice < 0)
            return BadRequest("Price cannot be negative");
        
        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id.Equals(id));
        
        if(product == null)
            return NotFound();
        
        // keep track of old values
        var oldPrice = product.Price;
        var oldLastUpdatedAt = product.LastUpdatedAt;

        // update new values
        product.Price = newPrice;
        product.LastUpdatedAt = DateTime.Now;
        
        // log changes
        Console.WriteLine("Old price: " + oldPrice + ", Old LastUpdatedAt: " + oldLastUpdatedAt +
                          ", New Price: " + product.Price + ", New LastUpdatedAt: " + product.LastUpdatedAt);
        
        return Ok(product);
    }

    [HttpPost("{id}/image")]
    // removed [FromQuery] and [FromForm] since they caused runtime errors
    public IActionResult UploadImage(string id, IFormFile image)
    {
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
        // ID should match GUID format
        if (id?.Length != 36)
            return BadRequest("Invalid ID format");

        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id.Equals(id));

        if (product == null)
            return NotFound();

        product.IsArchived = true;
        return Ok(product);
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