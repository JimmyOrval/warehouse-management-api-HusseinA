using FluentValidation;

namespace Application.Features.WarehouseItems.Commands.DeleteWarehouseItem;

public class DeleteWarehouseItemCommandValidator : AbstractValidator<DeleteWarehouseItemCommand>
{
    public DeleteWarehouseItemCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Item ID is required")
            .Length(36)
            .WithMessage("Item ID format invalid");
    }
}