using Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Policy = "AuthenticatedUser")]
[ApiController]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
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