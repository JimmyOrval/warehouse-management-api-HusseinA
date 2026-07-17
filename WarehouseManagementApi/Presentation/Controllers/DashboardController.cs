using Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetDashboardQuery(), cancellationToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                Message = "The operation failed!", 
                ActualErrorText = result.Error
            });
        }

        return Ok(result.Value);
    }
}