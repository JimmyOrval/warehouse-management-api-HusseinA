using Application.ViewModels;
using MediatR;

namespace Application.Features.Notifications.Queries.ListNotifications;

public record ListNotificationsQuery : IRequest<List<NotificationViewModel>>;