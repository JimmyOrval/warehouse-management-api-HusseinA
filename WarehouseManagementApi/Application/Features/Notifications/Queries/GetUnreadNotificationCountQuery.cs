using Application.ViewModels;
using MediatR;

namespace Application.Features.Notifications.Queries;

public record GetUnreadNotificationCountQuery
    : IRequest<UnreadNotificationCountViewModel>;