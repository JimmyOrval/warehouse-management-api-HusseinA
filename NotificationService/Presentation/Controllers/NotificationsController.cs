using Application.Features.Notifications.Commands.MarkNotificationAsRead;
using Application.Features.Notifications.Queries.ListNotifications;
using Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AuthenticatedUser")]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ListNotificationsQuery(), cancellationToken));
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkNotificationAsReadCommand(id), cancellationToken);
        return NoContent();
    }
}