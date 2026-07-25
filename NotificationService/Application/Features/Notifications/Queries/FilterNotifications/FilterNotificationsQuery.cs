using Application.ViewModels;
using Domain.Enums;
using MediatR;

namespace Application.Features.Notifications.Queries.FilterNotifications;

public record FilterNotificationsQuery(
    NotificationType? Type = null,
    NotificationSeverity? Severity = null,
    NotificationStatus? Status = null)
    : IRequest<List<NotificationViewModel>>;