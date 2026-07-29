using MediatR;

namespace Application.Features.Notifications.Queries.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery() : IRequest<int>;