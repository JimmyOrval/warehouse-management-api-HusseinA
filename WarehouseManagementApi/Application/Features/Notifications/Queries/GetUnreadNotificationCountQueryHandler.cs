using Application.ViewModels;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Notifications.Queries;

public class GetUnreadNotificationCountQueryHandler(
    INotificationServiceClient client)
    : IRequestHandler<GetUnreadNotificationCountQuery,
        UnreadNotificationCountViewModel>
{
    public async Task<UnreadNotificationCountViewModel> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var count = await client.GetUnreadNotificationCountAsync(cancellationToken);
        return new UnreadNotificationCountViewModel(count, count != null);
    }
}