using FluentValidation;

namespace Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage("Notification ID required")
            .Length(36)
            .WithMessage("Notification ID format invalid");
    }
}