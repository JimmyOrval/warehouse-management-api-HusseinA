using Microsoft.AspNetCore.Mvc;
using WarehouseManagementApi.Contracts;
using WarehouseManagementApi.Models;
using WarehouseManagementApi.Services;

namespace WarehouseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// SupplierService is injected as a primary constructor
public class SuppliersController(ISuppliersService suppliersService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetSuppliers()
    {
        // methods simply call the service instead of containing the logic themselves
        return Ok(suppliersService.GetSuppliers());
    }

    [HttpGet("{id}")]
    public IActionResult GetSupplier(string id)
    {
        if (id.Length != 36)
            return BadRequest("Invalid ID format");

        var supplier = suppliersService.GetSupplier(id);
        
        if(supplier == null)
            return NotFound();
        
        return Ok(supplier);
    }

    [HttpPost]
    public IActionResult CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var supplier = suppliersService.CreateSupplier(request);
        return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSupplier(string id)
    {
        if (id.Length != 36)
            return BadRequest("Invalid ID format");

        var supplier = suppliersService.DeleteSupplier(id);

        if (supplier == null)
            return NotFound();

        return Ok();
    }
}