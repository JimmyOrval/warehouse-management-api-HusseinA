using Application.Features.Notifications.Commands.MarkNotificationAsRead;
using Application.Features.Notifications.Queries.FilterNotifications;
using Application.Features.Notifications.Queries.GetNotificationById;
using Application.Features.Notifications.Queries.GetUnreadNotificationCount;
using Application.Features.Notifications.Queries.ListNotifications;
using Application.ViewModels;
using Domain.Enums;
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

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] NotificationType? type,
        [FromQuery] NotificationSeverity? severity,
        [FromQuery] NotificationStatus? status,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new FilterNotificationsQuery(
            type, severity, status), cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetNotificationByIdQuery(id), cancellationToken));
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotificationCount(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetUnreadNotificationCountQuery(), cancellationToken));
    }
}