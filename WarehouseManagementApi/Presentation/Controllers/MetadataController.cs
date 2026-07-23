using Application.Features.Products.Commands.CreateProduct;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Reflection;

namespace Presentation.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
    [HttpGet("product")]
    public IActionResult GetProductMetadata()
    {
        return Ok(
            ValidationMetadataHelper
                .GetValidationMetadata<Product>());
    }
}