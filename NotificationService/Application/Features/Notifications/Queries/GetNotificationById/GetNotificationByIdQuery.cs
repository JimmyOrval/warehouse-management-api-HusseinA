using Application.ViewModels;
using MediatR;

namespace Application.Features.Notifications.Queries.GetNotificationById;

public record GetNotificationByIdQuery(string Id) : IRequest<NotificationViewModel>;