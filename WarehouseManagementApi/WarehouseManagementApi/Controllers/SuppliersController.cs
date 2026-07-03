using Microsoft.AspNetCore.Mvc;
using WarehouseManagementApi.Contracts;
using WarehouseManagementApi.Models;

namespace WarehouseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSuppliers()
    {
        return Ok(FakeSupplierDirectory.Suppliers.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetSupplier(string id)
    {
        if (id.Length != 36)
            return BadRequest("Invalid ID format");
        
        var supplier = FakeSupplierDirectory.Suppliers
            .FirstOrDefault(s => s.Id.Equals(id));
        
        if(supplier == null)
            return NotFound();
        
        return Ok(supplier);
    }

    [HttpPost]
    public IActionResult CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name =  request.Name,
            Country =  request.Country,
            ContactEmail = request.ContactEmail,
            Phone =  request.Phone,
            IsActive = true
        };
        
        FakeSupplierDirectory.Suppliers.Add(supplier);
        return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSupplier(string id)
    {
        if (id.Length != 36)
            return BadRequest("Invalid ID format");

        var supplier = FakeSupplierDirectory.Suppliers
            .FirstOrDefault(s => s.Id.Equals(id));

        if (supplier == null)
            return NotFound();

        supplier.IsActive = false;
        return Ok(supplier);
    }
}