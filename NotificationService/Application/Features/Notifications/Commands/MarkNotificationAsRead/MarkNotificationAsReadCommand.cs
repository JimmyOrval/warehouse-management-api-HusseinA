using Application.ViewModels;
using MediatR;

namespace Application.Features.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(string Id) : IRequest<NotificationViewModel>;