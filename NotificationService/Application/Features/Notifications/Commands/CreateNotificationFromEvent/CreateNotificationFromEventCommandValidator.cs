using FluentValidation;

namespace Application.Features.Notifications.Commands.CreateNotificationFromEvent;

public class CreateNotificationFromEventCommandValidator
    : AbstractValidator<CreateNotificationFromEventCommand>
{
    public CreateNotificationFromEventCommandValidator()
    {
        RuleFor(n => n.EventId).NotEmpty();
        RuleFor(n => n.Title)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(n => n.Message)
            .NotEmpty()
            .MaximumLength(1000);
        RuleFor(n => n.Type).NotEmpty();
        RuleFor(n => n.Severity).NotEmpty();
        RuleFor(n => n.RelatedEntityId).NotEmpty();
        RuleFor(n => n.RelatedEntity).NotEmpty();
    }
}