using Microsoft.AspNetCore.Mvc;
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
            return BadRequest();
        
        var product = FakeWarehouseStore.Products.FirstOrDefault(x => x.Id.Equals(id));

        if (product == null)
            return NotFound();
        
        return Ok(product);
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? name, [FromQuery] string? supplier)
    {
        if(string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
            return BadRequest("Both filters empty. Please enter at least one.");

        var filteredProducts = FakeWarehouseStore.Products.AsEnumerable();

        if(!string.IsNullOrWhiteSpace(name))
        {
            filteredProducts = filteredProducts.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if(!string.IsNullOrWhiteSpace(supplier))
        {
            filteredProducts = filteredProducts.Where(p => p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase));
        }
        
        return Ok(filteredProducts.ToList());
    }
}